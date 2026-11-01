using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

// Personal-space experiment: a virtual person walks toward the participant,
// who pulls the right trigger when the person feels "too close".
// Attach this to an empty GameObject called "ExperimentManager".
public class ExperimentManager : MonoBehaviour
{
    [Header("Scene objects (drag these in from the Hierarchy)")]
    public Walker walker;            // the virtual person
    public TextMesh messageText;     // 3D text floating in front of the participant

    [Header("Design")]
    public float[] speeds = { 0.6f, 1.0f, 1.4f };   // walking speed, m/s
    public float[] lateralOffsets = { 0f, 0.6f };   // 0 = head-on, 0.6 = passes to one side, m
    public int repetitions = 4;                      // 3 speeds x 2 offsets x 4 reps = 24 trials
    public float startDistance = 6f;                 // walker's starting distance, m
    public float walkerHeight = 0f;                  // y position of the walker's feet

    [Header("Participant")]
    public string participantID = "P01";

    InputDevice device;
    bool triggerWasDown = false;
    bool primaryWasDown = false;
    string filename;

    struct Condition { public float speed; public float offset; }
    List<Condition> trials = new List<Condition>();

    void Start()
    {
        // Build and shuffle the trial list.
        foreach (float s in speeds)
            foreach (float o in lateralOffsets)
                for (int r = 0; r < repetitions; r++)
                    trials.Add(new Condition { speed = s, offset = o });
        Shuffle(trials);

        // Data file. persistentDataPath works both on PC and on a standalone Quest.
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        filename = Path.Combine(Application.persistentDataPath, $"{participantID}_{timestamp}.csv");
        using (StreamWriter writer = new StreamWriter(filename, true))
            writer.WriteLine("participant,trial,speed,offset,side,responded,distance_m,time_s");
        Debug.Log($"Saving data to {filename}");

        walker.Hide();
        StartCoroutine(RunExperiment());
    }

    IEnumerator RunExperiment()
    {
        for (int t = 0; t < trials.Count; t++)
        {
            Condition c = trials[t];

            // Self-paced start: wait for the A button (or space bar on the desktop).
            ShowMessage($"Trial {t + 1} of {trials.Count}\nPress A to start");
            yield return new WaitUntil(() => PrimaryPressed());
            ShowMessage("");

            // Short random delay so the onset isn't predictable.
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

            // Work out the walker's path relative to where the participant is looking now.
            Transform head = Camera.main.transform;
            Vector3 headFlat = new Vector3(head.position.x, walkerHeight, head.position.z);
            Vector3 forward = head.forward; forward.y = 0f; forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            int side = Random.value < 0.5f ? -1 : 1;   // pass on the left or the right
            Vector3 start = headFlat + forward * startDistance + right * c.offset * side;

            walker.Begin(start, -forward, c.speed);
            float startTime = Time.time;

            // Walk until the participant responds or the walker has gone past them.
            bool responded = false;
            float distance = float.NaN;
            while (true)
            {
                if (TriggerPressed())
                {
                    responded = true;
                    distance = HorizontalDistance(Camera.main.transform.position, walker.transform.position);
                    break;
                }
                // Stop once the walker is level with the participant (no response).
                Vector3 toWalker = walker.transform.position - headFlat;
                if (Vector3.Dot(toWalker, forward) < 0.2f)
                    break;
                yield return null;
            }
            float rt = Time.time - startTime;
            walker.Stop();

            string dataline = $"{participantID},{t + 1},{c.speed},{c.offset},{side},{(responded ? 1 : 0)},{distance:F3},{rt:F3}";
            using (StreamWriter writer = new StreamWriter(filename, true))
                writer.WriteLine(dataline);
            Debug.Log(dataline);

            yield return new WaitForSeconds(0.5f);
            walker.Hide();
            yield return new WaitForSeconds(0.5f);
        }

        ShowMessage("Done - thank you!");
        Debug.Log($"Experiment finished. Data saved to {filename}");
    }

    // ---------- helpers ----------

    void Update()
    {
        // Keep trying to find the right-hand controller (as in course notes 4.2).
        if (!device.isValid)
            device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Keep the message text in front of the participant's face.
        if (messageText != null && Camera.main != null)
        {
            Transform head = Camera.main.transform;
            messageText.transform.position = head.position + head.forward * 2f;
            messageText.transform.rotation = Quaternion.LookRotation(head.forward);
        }
    }

    // The XR method returns true for as long as a button is held,
    // so these functions return true only on the frame it is first pressed.
    bool TriggerPressed()
    {
        bool down = false;
        if (device.isValid)
            device.TryGetFeatureValue(CommonUsages.triggerButton, out down);
        bool pressed = down && !triggerWasDown;
        triggerWasDown = down;
        return pressed || Input.GetKeyDown(KeyCode.Return);   // Enter key for desktop testing
    }

    bool PrimaryPressed()
    {
        bool down = false;
        if (device.isValid)
            device.TryGetFeatureValue(CommonUsages.primaryButton, out down);
        bool pressed = down && !primaryWasDown;
        primaryWasDown = down;
        return pressed || Input.GetKeyDown(KeyCode.Space);    // space bar for desktop testing
    }

    float HorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f; b.y = 0f;
        return Vector3.Distance(a, b);
    }

    void ShowMessage(string msg)
    {
        if (messageText != null)
            messageText.text = msg;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
