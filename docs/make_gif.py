"""Pendulum アプリ (Form1/EquationSolver/Stand) と同じ式・同じパラメータで
振り子を再計算し、アプリの描画 (DrawSystem) と同じ見た目で GIF を書き出す。"""
import math
import sys
from PIL import Image, ImageDraw, ImageFont

OUT = sys.argv[1]

# Form1: vl = new Value(0.01, 9.8, 0.0001, 150.0, 150deg, 0.0)
m, g, k, r = 0.01, 9.8 * 1000, 0.0001, 150.0
INTERVAL = 0.017          # timer1.Interval = 17 ms
H = INTERVAL / 100        # 刻み幅 h


def simulate(omg, th0, frames, B=100.0, x0=250.0, y0=200.0):
    """Stand(250, 200, 100, omg) + EquationSolver.Euler(mode=0) の再現"""
    th, dth, t = th0, 0.0, 0.0
    out = []
    for _ in range(frames):
        t += INTERVAL
        sx = x0 + B * math.sin(omg * t)            # Stand.setValue
        out.append((t, sx, y0, th))                # 描画 → その後に次の角度計算
        tt = t
        for _ in range(int(round(INTERVAL / H))):
            th += H * dth
            acc = (B * omg * omg * math.sin(omg * tt) * math.cos(th) / r
                   - g * math.sin(th) / r - k * dth / m)
            dth += H * acc
            tt += H
    return out


def font(size):
    for name in ("meiryo.ttc", "YuGothM.ttc", "msgothic.ttc", "arial.ttf"):
        try:
            return ImageFont.truetype(name, size)
        except OSError:
            pass
    return ImageFont.load_default()


F = font(15)
FS = font(12)


def panel(state, title, sub):
    """DrawSystem と同じ: 白背景, 幅200×高さ10 の台, 半径10 の錘, 糸"""
    t, sx, sy, th = state
    W, Hh = 500, 400
    im = Image.new("RGB", (W, Hh), "white")
    d = ImageDraw.Draw(im)
    d.rectangle([sx - 100, sy - 5, sx + 100, sy + 5], outline="black")
    px, py = sx + r * math.sin(th), sy + r * math.cos(th)
    d.line([sx, sy, px, py], fill="black")
    d.ellipse([px - 10, py - 10, px + 10, py + 10], outline="black", fill="white")
    d.text((10, 8), title, fill="black", font=F)
    d.text((10, 30), sub, fill=(90, 90, 90), font=FS)
    d.text((10, Hh - 24), f"t = {t:5.2f} s", fill=(90, 90, 90), font=FS)
    return im


def make(frames_spec, step, n):
    sims = [simulate(omg, th0, n * step) for _, _, omg, th0 in frames_spec]
    imgs = []
    for i in range(0, n * step, step):
        row = [panel(s[i], ti, su) for s, (ti, su, _, _) in zip(sims, frames_spec)]
        canvas = Image.new("RGB", (sum(p.width for p in row) + 10 * (len(row) + 1),
                                   row[0].height + 20), (200, 200, 200))
        x = 10
        for p in row:
            canvas.paste(p, (x, 10))
            x += p.width + 10
        canvas = canvas.resize((canvas.width * 3 // 5, canvas.height * 3 // 5),
                               Image.LANCZOS)
        imgs.append(canvas.convert("P", palette=Image.ADAPTIVE, colors=32))
    return imgs


w0 = math.sqrt(g / r)
spec = [
    ("共振: ω = ω0 = 8.08 rad/s", "台の振動数 = 振り子の固有振動数", 8.08, math.radians(30)),
    ("非共振: ω = 4.0 rad/s", "台の振動数 ≠ 振り子の固有振動数", 4.0, math.radians(30)),
]
STEP, N = 2, 300   # 2 ステップ(34 ms)ごとに 1 コマ, 約 10 秒
imgs = make(spec, STEP, N)
imgs[0].save(OUT, save_all=True, append_images=imgs[1:],
             duration=int(INTERVAL * STEP * 1000), loop=0, optimize=True)
print("omega0 =", w0, "frames =", len(imgs))
