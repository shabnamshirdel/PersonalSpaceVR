# Plot comfort distance by walking speed and path offset.
# Usage: python analysis/analyze.py P01_2026-11-02_14-30-00.csv [more files...]
import sys
import pandas as pd
import matplotlib.pyplot as plt

df = pd.concat([pd.read_csv(f) for f in sys.argv[1:]])
df = df[df.responded == 1]
summary = df.groupby(["offset", "speed"]).distance_m.agg(["mean", "sem"]).reset_index()

fig, ax = plt.subplots(figsize=(5, 4))
for offset, g in summary.groupby("offset"):
    label = "head-on" if offset == 0 else f"{offset} m to the side"
    ax.errorbar(g.speed, g["mean"], yerr=g["sem"], marker="o", capsize=3, label=label)
ax.set_xlabel("Walking speed (m/s)")
ax.set_ylabel("Comfort distance (m)")
ax.set_title("When does a virtual person feel too close?")
ax.legend(frameon=False)
ax.spines[["top", "right"]].set_visible(False)
fig.tight_layout()
fig.savefig("comfort_distance.png", dpi=200)
print(summary)
