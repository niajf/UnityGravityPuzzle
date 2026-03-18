# Gravity Puzzle

Unity 6 を用いて設計・開発した3D重力操作パズルゲームです。
マウスクリックで重力方向を6軸から切り替え、プレイヤーをゴールへと導くことがコンセプトです。ゲームプレイの面白さだけでなく、**拡張性・保守性を意識したアーキテクチャ設計**をポートフォリオとして示すことを目的としています。

## 🛠 技術スタック (Tech Stack)

| 項目 | 内容 |
|---|---|
| **Engine** | Unity 6 (6000.0.66f2) |
| **Language** | C# |
| **Physics** | Unity Physics Engine |
| **UI** | TextMeshPro |
| **Platform** | Windows / macOS |

## 🎮 ゲーム概要 (Game Overview)

- **操作:** WASD / 矢印キーで移動、マウスで視点操作
- **重力変更:** 左クリック → プレイヤー左方向、右クリック → プレイヤー右方向へ重力を切り替え
- **目標:** 重力を操りステージ上のゴールエリアへ到達する
- **障害:** キルゾーン、ステージ外への落下、追跡ドローン

## 🚀 技術的なこだわり (Technical Highlights)

### 1. イベント駆動アーキテクチャ (Event-Driven Architecture)

スクリプト間の依存を最小化するために、C# の `event` を軸としたイベント駆動設計を採用しました。

```
GravityManager.OnGravityChanged  →  PlayerController, UIManager, AntiGravityObject
GameFlowManager.OnGameClearOccurred / OnGameOverOccurred  →  UIManager
```

- 重力変更の通知を `OnGravityChanged` イベントで伝播させることで、`GravityManager` が他コンポーネントの存在を知らなくてよい疎結合な設計を実現。
- すべての購読者は `OnDestroy()` でイベントを解除し、**メモリリークを防止**しています。

### 2. シングルトンマネージャーによる責務分離 (Singleton Managers)

ゲームの状態・重力・サウンドを担う3つのシングルトンマネージャーで役割を明確に分離しています。

| マネージャー | 責務 |
|---|---|
| `GravityManager` | 重力方向の一元管理、`Physics.gravity` の適用 |
| `GameFlowManager` | `Playing` / `Cleared` / `GameOver` の状態遷移管理 |
| `SoundManager` | BGM・SEの再生管理 |

### 3. クォータニオンによる滑らかな姿勢制御 (Quaternion-based Orientation)

重力変更時にプレイヤーが瞬時に回転するのではなく、`Quaternion.Slerp` を用いて**新しい重力方向へ滑らかに姿勢を補間**しています。

```csharp
// PlayerController.cs — 重力方向へ向かって毎フレーム姿勢を補正
Quaternion gravityAligned = Quaternion.Slerp(
    rb.rotation,
    Quaternion.FromToRotation(transform.up, targetUpVector) * rb.rotation,
    rotateSpeed * Time.fixedDeltaTime
);
rb.MoveRotation(gravityAligned);
```

### 4. 内積を活用した重力方向スナッピング (Dot Product Gravity Snapping)

マウスで指定した方向に最も近い「6軸の単位ベクトル」を内積で選出する設計を採用しています。浮動小数点誤差に依存せず、常に軸対称な重力方向が選ばれる安定した実装です。

```csharp
// GravityManager.cs — 最も近い軸方向を内積で決定
foreach (Vector3 v in candidates)
{
    float dot = Vector3.Dot(v, newDirection);
    if (dot > maxDot) { maxDot = dot; GravityDirection = v; }
}
```

### 5. SphereCast によるカメラ壁抜け防止 (Camera Collision Avoidance)

サードパーソンカメラが壁に埋まることを防ぐため、`Physics.SphereCast` を用いてカメラとプレイヤーの間の障害物を検出し、**衝突時には即座に・引く際は Lerp で滑らかに**距離を調整しています。

```csharp
// CameraFollow.cs — 壁に近いときは即座に寄り、離れるときは滑らかに引く
if (targetDistance < currentDistance)
    currentDistance = targetDistance;           // 壁に近い → 即座に寄る
else
    currentDistance = Mathf.Lerp(currentDistance, targetDistance, smoothSpeed * Time.deltaTime);
```

### 6. ドローンAI：ステートマシン × 視野判定 (Drone AI)

ドローン敵を **距離 → 視野角 → Raycast 障害物** の3段階フィルタで構成した視界判定システムで実装しました。

1. **距離判定:** `magnitude` で索敵範囲外を早期リターン
2. **視野角判定:** `Vector3.Angle` で前方 FOV の外をカット
3. **障害物判定:** `Physics.Raycast` で遮蔽物を考慮した視線チェック

Inspector 上でデバッグしやすいよう、`OnDrawGizmosSelected` で視界範囲・視野角を**エディタ上にリアルタイムで可視化**しています。

### 7. UnityEvent による柔軟なトリガー設計 (Inspector-Driven Events)

`PressureSwitch` はドアの開閉ロジックを直接持たず、`UnityEvent` (`onActivate` / `onDeactivate`) を Inspector でワイヤリングする設計にしました。コードを変更せずにレベルデザイナーが任意のアクションをバインドできます。

## 📂 ディレクトリ構成 (Project Structure)

```
Assets/Scripts/
├── Core/
│   ├── GravityManager.cs      # 重力の一元管理
│   ├── GameFlowManager.cs     # ゲーム状態遷移
│   └── SoundManager.cs        # サウンド管理
├── Player/
│   ├── PlayerController.cs    # 移動・姿勢制御
│   └── CameraFollow.cs        # サードパーソンカメラ
├── Enemy/
│   └── DroneAI.cs             # ステートマシン敵AI
├── Stage/
│   ├── GoalArea.cs            # ゴール判定
│   ├── KillZone.cs            # 即死ゾーン
│   ├── OutStage.cs            # ステージ外脱出検知
│   ├── PressureSwitch.cs      # UnityEventトリガー
│   └── Door.cs                # Lerp開閉アニメーション
├── UI/
│   ├── UIManager.cs           # HUD・パネル制御
│   ├── CanvasGroupFader.cs    # フェードイン/アウト
│   ├── UIFloatEffect.cs       # 浮遊アニメーション
│   └── ButtonOutlineEffect.cs # ボタン演出
└── Title/
    ├── TitleManager.cs        # タイトル画面制御
    └── TitleBackground.cs     # タイトル背景演出
```

## 🎯 実装機能 (Features)

- **重力操作システム:** 6方向への重力切り替えと物理演算への即時反映
- **姿勢補間:** 重力変更に連動したプレイヤーの滑らかな回転
- **サードパーソンカメラ:** SphereCast による壁抜け防止・縦横マウス操作
- **ドローン敵AI:** 3段階の視界判定と Idle / Chase ステートマシン
- **ステージギミック:** 圧力スイッチ連動ドア（Lerp アニメーション）
- **ゴール・死亡判定:** コライダートリガーによるゲーム状態遷移
- **HUD:** TextMeshPro による重力方向表示・クリアタイム計測
- **タイトル画面:** フェードイン、浮遊アニメーション演出
