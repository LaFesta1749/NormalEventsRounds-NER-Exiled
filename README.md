# 🎮 NormalEventsRounds (NER)
A plugin for **SCP: Secret Laboratory** using **Exiled 9.5.1** that tracks and announces event winners via Discord Webhook.

## 📌 Features
✅ Tracks player victories in **three different event categories**  
✅ Sends **Discord webhook notifications** with embedded messages  
✅ Supports **configurable win thresholds** (e.g., `5/5` wins)  
✅ Automatically pauses event notifications when a player reaches the max wins  
✅ Includes a **Remote Admin (RA) command** to reset winners  

---

## 🛠️ Installation
1. **Download the latest release** from [GitHub Releases](https://github.com/yourrepo/NER/releases).
2. Place the `NER.dll` file inside your **Exiled Plugins** folder:

Exiled/ │ ├── Plugins/ │ │ ├── NER.dll

3. **Restart the server** to generate the `config.yml` file.

---

## ⚙️ Configuration (`config.yml`)
After the first server restart, a `config.yml` file will be created in:

Exiled/Configs/port-config.yml


### **Default Configuration:**
```yaml
is_enabled: true
debug: false
webhook_url: "https://your-webhook-url-here"
wins_required: 5
```

## Options:
```yaml
is_enabled	Enables/disables the plugin	true
debug	Enables debug logs in the console	false
webhook_url	Discord Webhook URL for event notifications	"your-webhook-url"
wins_required	Wins required before notifications pause (reward)	5
```

## 🎮 Events (Normal Games):
1️⃣ Last Man Standing - If only one player is alive at the end of the round, they win.
2️⃣ SCP Hunter - The player who dealt the most SCP damage wins.
3️⃣ Elite Escape - The first escaping Class-D or Scientist wins.

🔹 Priority Order:
If multiple events qualify, the winner is determined in this order:
1️⃣ Last Survivor → 2️⃣ SCP Hunter → 3️⃣ First Escape

## 📢 Webhook Messages
The plugin sends Discord webhook messages when a player wins an event or reaches the max wins.

1️⃣ Example Win Notification
```yaml
{
  "username": "Event System",
  "embeds": [
    {
      "title": "🎉 [EVENT WINNER] - SCP Hunter",
      "description": "**ValkyrieVibe** won the event: **SCP Hunter**!\n🏆 **ValkyrieVibe** now has **3/5** wins!",
      "color": 16753920,
      "fields": [
        {
          "name": "📊 Next Top 3 Players:",
          "value": "🥈 **Player2** - 4 wins\n🥉 **Player3** - 3 wins\n🏅 **Player4** - 2 wins",
          "inline": false
        }
      ],
      "footer": {
        "text": "SCP:SL Event System"
      }
    }
  ]
}
```

## 2️⃣ Example Reward Notification (Max Wins)
```yaml
{
  "username": "Event System",
  "content": "<@902836984102395915> <@294860832297320458>", 
  "embeds": [
    {
      "title": "🎁 [EVENT]",
      "description": "**PlayerName** has reached **5/5 wins** and should receive a reward!\n\n⚠ Webhook notifications are paused until the winners have been cleared.",
      "color": 65280,
      "footer": {
        "text": "SCP:SL Event System"
      }
    }
  ]
}
```

## 🛠 Commands
The plugin includes a Remote Admin (RA) command to reset winners manually.

🔹 Reset Winners
`ner_reset`
✅ Effect: Clears all event winners and allows event tracking to continue.

## 🎯 How It Works
1. Players compete in events (Last Man Standing, SCP Hunter, Elite Escape).
2. Winners are tracked and stored in winners.yml.
3. Webhook messages are sent when a player wins.
4. When a player reaches 5/5 wins, notifications pause, and admins are pinged.
5. Admins use ner_reset to restart event tracking.

### 🚀 Planned Features
✅ Log event wins to a separate file
✅ Configurable event rewards
✅ More event types (e.g., Most Kills, Fastest Escape)
✅ Per-player statistics tracking
