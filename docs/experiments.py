"""README「考察」の数値実験。
アプリ (EquationSolver.Euler, mode 0) と同じ式・同じ刻み幅で計算する。
実行: python docs/experiments.py   （数分かかります）"""
import math
import numpy as np

m, g, k, r = 0.01, 9800.0, 0.0001, 150.0   # kg, mm/s^2, kg/s, mm
INTERVAL = 0.017                           # タイマー間隔 [s]
H = INTERVAL / 100                         # 刻み幅 [s]
W0 = math.sqrt(g / r)                      # 固有角振動数


def run(omg, th0, T, B=100.0, kk=k, cromer=True):
    """cromer=True: アプリと同じ (θ を先に更新し、新しい θ で角加速度を計算)
       cromer=False: 教科書どおりの前進オイラー法"""
    th, dth, t = th0, 0.0, 0.0
    ts, ths = [], []
    for _ in range(int(T / INTERVAL)):
        t += INTERVAL
        ts.append(t)
        ths.append(th)
        tt = t
        for _ in range(100):
            if cromer:
                th += H * dth
                acc = (B * omg**2 * math.sin(omg * tt) * math.cos(th) / r
                       - g * math.sin(th) / r - kk * dth / m)
            else:
                acc = (B * omg**2 * math.sin(omg * tt) * math.cos(th) / r
                       - g * math.sin(th) / r - kk * dth / m)
                th += H * dth
            dth += H * acc
            tt += H
    return np.array(ts), np.array(ths)


print(f"固有角振動数 ω0 = {W0:.3f} rad/s, 周期 T0 = {2 * math.pi / W0:.4f} s")

print("\n[1] 積分法の違い (台を止め、減衰なし、初期角度 30°, 60 秒)")
for c, name in ((True, "アプリの方法 (半陰的オイラー)"), (False, "前進オイラー法")):
    _, th = run(1, math.radians(30), 60, B=0, kk=0, cromer=c)
    print(f"  {name}: 最初の振幅 {math.degrees(th[:120].max()):.1f}°"
          f" → 60 秒後 {math.degrees(th[-120:].max()):.1f}°")

print("\n[2] 振幅と周期 (台を止め、減衰なし)")
for deg in (10, 30, 90, 150, 170):
    ts, th = run(1, math.radians(deg), 20, B=0, kk=0)
    s = np.where((th[:-1] > 0) & (th[1:] <= 0))[0]
    T = np.mean(np.diff(ts[s]))
    print(f"  振幅 {deg:3d}°: 周期 {T:.3f} s  (T/T0 = {T / (2 * math.pi / W0):.2f})")

print("\n[3] 共振曲線 (台の振幅 B = 2 mm, 静止から 200 秒, 最大振れ角)")
for om in (7.0, 7.5, 7.7, 7.8, 7.9, 8.0, 8.08, 8.2, 8.4, 9.0):
    _, th = run(om, 0.0, 200, B=2.0)
    print(f"  ω = {om:5.2f}: {math.degrees(np.abs(th).max()):5.1f}°")

print("\n[4] 台の振幅 B = 100 mm (アプリの既定値), 初期角度 30°, 30 秒")
for om in (4, 6, 8.08, 8.3, 10, 12):
    _, th = run(om, math.radians(30), 30)
    print(f"  ω = {om:5.2f}: 最大 |θ| = {np.abs(th).max():6.2f} rad")

print("\n[5] 初期値鋭敏性 (B = 100 mm, ω = 8.3, 初期角度 30° と 30.001°)")
_, a = run(8.3, math.radians(30), 30)
_, b = run(8.3, math.radians(30.001), 30)
for T in (5, 10, 15, 20, 30):
    i = int(T / INTERVAL) - 1
    print(f"  {T:2d} 秒後の角度の差: {abs(a[i] - b[i]):.5f} rad")
