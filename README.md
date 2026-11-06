# Pendulum — 強制振動する台に吊るした振り子の共振シミュレータ

左右に振動する台から振り子を吊るし、その運動を数値計算してアニメーション表示する
C# / Windows Forms アプリです。台の振動数を振り子の固有振動数に合わせると、
振り子が**共振**して振れ幅がどんどん大きくなる様子を観察できます。

高専5年のときに、学習用として製作しました。
教科書に出てくる力学の問題（強制振動・共振）を、式で解くだけでなく
実際に動く様子として可視化してみたかったのが製作のきっかけです。

![共振と非共振の比較](docs/pendulum.gif)

> 左: 台の角振動数 ω を振り子の固有角振動数 ω₀ ≈ 8.08 rad/s に合わせた場合（共振）。
> 右: ω = 4.0 rad/s の場合（非共振）。どちらも初期角度 30°。
> この GIF はアプリと同じ運動方程式・同じパラメータ・同じ描画方法で
> [docs/make_gif.py](docs/make_gif.py) が再計算して作ったものです。

---

## モデル

### 台（[Stand.cs](Pendulum/Stand.cs)）

台は水平方向に単振動します（単位は mm）。

$$
x_s(t) = x_0 + B \sin(\omega t), \qquad x_0 = 250,\ B = 100
$$

### 振り子の運動方程式（[EquationSolver.cs](Pendulum/EquationSolver.cs)）

台と一緒に動く座標系で見ると、振り子には重力・粘性抵抗のほかに
台の加速度による**慣性力**がはたらきます。

$$
\ddot\theta
= \underbrace{\frac{B\omega^2 \sin(\omega t)\cos\theta}{r}}_{\text{慣性力}}
\; \underbrace{-\,\frac{g}{r}\sin\theta}_{\text{重力}}
\; \underbrace{-\,\frac{k}{m}\dot\theta}_{\text{粘性抵抗}}
$$

これを**オイラー法**で解いています。タイマー 1 回（17 ms）の間に
刻み幅 h = 0.17 ms で 100 ステップ進めます。

| 記号 | 意味 | 既定値 |
|---|---|---|
| m | 錘の質量 | 0.01 kg |
| g | 重力加速度 | 9.8 m/s²（内部では ×1000 して mm/s²） |
| k | 粘性係数 | 0.0001 kg/s |
| r | 糸の長さ | 150 mm |
| θ₀ | 初期角度 | 150°（2.618 rad） |
| ω | 台の角振動数 | 8.3 rad/s |
| B | 台の振幅 | 100 mm |

固有角振動数は ω₀ = √(g/r) = √(9800/150) ≈ **8.08 rad/s** です。
既定値の ω = 8.3 rad/s は ω₀ のすぐ近くなので、共振に近い状態になっています。

---

## 使い方

1. Visual Studio で [Pendulum.sln](Pendulum.sln) を開く（.NET Framework 4.7.2）
2. ビルドして実行（ビルド済みの exe は `Pendulum/bin/Debug/Pendulum.exe`）

| ボタン | 動作 |
|---|---|
| **Start** | 初期状態からシミュレーションをやり直す |
| **Stop / Resume** | 一時停止・再開 |
| **Option** | パラメータ設定ウィンドウ（Form2）を開く |

Option ウィンドウでは質量・重力加速度・粘性係数・半径・初期角度・初期角速度を変更できます。
「送信」を押したあと、メイン画面で **Start** を押すと新しい値で計算が始まります。

実行中は Visual Studio の出力ウィンドウに `時刻,角度[rad]` が 1 行ずつ出力されます。
同梱の CSV はこの出力を保存したものです。

---

## ファイル構成

```
Pendulum/
├─ Pendulum.sln
├─ Pendulum/                 アプリ本体
│  ├─ Program.cs             エントリポイント（Form1 を起動）
│  ├─ Form1.cs               メイン画面: 描画・タイマー・ボタン、Value 構造体（系のパラメータ）
│  ├─ Form2.cs               パラメータ設定画面
│  ├─ Pendulum.cs            振り子の角度・座標を保持し、EquationSolver で次の角度を計算
│  ├─ Stand.cs               振動する台の位置と加速度
│  └─ EquationSolver.cs      運動方程式とオイラー法
├─ sample.cs                 Form1.cs の旧バージョン（Shift-JIS、ビルド対象外）
├─ docs/
│  ├─ pendulum.gif           README の GIF
│  └─ make_gif.py            GIF を作る Python スクリプト
└─ 計算結果のデータ（下表）
```

### データファイル

| ファイル | 内容 |
|---|---|
| `kyoushin.csv` / `.xlsx` / `.png` | 共振条件での `時刻[s], 角度[rad]` とグラフ |
| `hikyoushin.csv` / `.xlsx` / `.png` | 非共振条件での `時刻[s], 角度[rad]` とグラフ |
| `data.csv`, `data (2).csv` | 別条件で出力した `時刻[s], 角度[rad]` |
| `xa.csv` / `.xlsx` | `時刻, 台の位置 x, 台の加速度（差分による数値計算）, 台の加速度（解析解）` — 加速度の計算方法の比較 |
| `data2.csv` / `.xlsx` | 角度データのフーリエ係数（a₀, aₖ, bₖ）の計算表 |

> 共振のグラフで角度が 20 rad を超えているのは、振り子が支点の周りを
> 何周も回転しているためです（角度を ±π に折り返していません）。

---

## 補足・既知の問題

- `Form2` のフィールド初期化子 `public Form1 f = new Form1();` により、Option を開くたびに
  表示されない Form1（タイマー付き）が裏で 1 つ作られます。`public Form1 f;` にすれば解消します。
- Option で数値以外を入力して「送信」を押すと `double.Parse` で例外になります。
- 台の振動数 ω と振幅 B は `Form1.Initialize()` 内の `new Stand(250.0, 200.0, 100.0, 8.3)`
  で固定されており、Option 画面からは変更できません。
- `EquationSolver.ddfunc` には、台の加速度を解析式で与える版（mode 0、使用中）と
  位置の差分から求める版（mode 1）の 2 種類があります。
