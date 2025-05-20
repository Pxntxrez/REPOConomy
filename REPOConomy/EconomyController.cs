using Photon.Pun;
using UnityEngine;

namespace REPOConomyMod
{
    public class EconomyController : MonoBehaviour
    {
        public static EconomyController Instance;
        public static bool UIShownOnce = false;
        private bool economyResetPending = true;
        private bool showGUI = false;
        private float displayTimer = 6f;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                showGUI = !showGUI;
                displayTimer = 6f;
            }

            if (economyResetPending && SemiFunc.RunIsLevel())
            {
                REPOConomyPlugin.ResetEconomy();
                economyResetPending = false;
            }

            if (!SemiFunc.RunIsLevel())
            {
                economyResetPending = true;
                UIShownOnce = false;
            }

            if (showGUI && REPOConomyPlugin.HasEconomy && SemiFunc.RunIsLevel())
            {
                displayTimer -= Time.deltaTime;
                if (displayTimer <= 0f)
                    showGUI = false;
            }
        }

        public void ResetGUI()
        {
            showGUI = true;
            displayTimer = 6f;
        }

        public void ShowGUI()
        {
            showGUI = true;
            displayTimer = 6f;
        }

        private float ClampToStep(float value, float step = 0.1f)
        {
            return Mathf.Round(value / step) * step;
        }

        private void OnGUI()
        {
            if (!showGUI || !REPOConomyPlugin.HasEconomy || !SemiFunc.RunIsLevel())
                return;

            int level = REPOConomyPlugin.GetCurrentLevel();
            float deltaMin;
            float deltaMax;

            if (PhotonNetwork.IsMasterClient)
            {
                float scaleMin = ClampToStep(REPOConfig.LevelScaleMin.Value);
                float scaleMax = ClampToStep(REPOConfig.LevelScaleMax.Value);

                deltaMin = (level - 1) * scaleMin;
                deltaMax = (level - 1) * scaleMax;
            }
            else
            {
                deltaMin = REPOConomyPlugin.ClientDeltaMin;
                deltaMax = REPOConomyPlugin.ClientDeltaMax;
            }

            string econName = Photon.Pun.PhotonNetwork.IsMasterClient
                ? REPOConomyPlugin.GetEconomyName()
                : REPOConomyPlugin.CurrentEconomyName;
            string min = REPOConomyPlugin.MinPercent.ToString("+#0.##;-#0.##;0");
            string max = REPOConomyPlugin.MaxPercent.ToString("+#0.##;-#0.##;0");
            string econText = $"Economy: {econName}";
            string rangeText = $"Range: {min}% - {max}%";
            string levelText =
                (Mathf.Approximately(deltaMin, deltaMax))
                    ? $"Level: {level} ({deltaMin:+0.##;-0.##;0}%)"
                    : $"Level: {level} ({deltaMin:+0.##;-0.##;0}% {deltaMax:+0.##;-0.##;0}%)";
            string econDesc = Photon.Pun.PhotonNetwork.IsMasterClient
                ? REPOConomyPlugin.GetEconomyFlavor()
                : REPOConomyPlugin.CurrentEconomyFlavor;

            float centerX = Screen.width / 2f;
            float topY = Screen.height * 0.15f;

            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 26,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter,
                richText = true,
                wordWrap = true,
            };

            style.normal.textColor = REPOConomyPlugin.GetEconomyColor();

            float labelWidth = Mathf.Clamp(
                Mathf.Max(
                    style.CalcSize(new GUIContent(econText)).x,
                    style.CalcSize(new GUIContent(rangeText)).x,
                    style.CalcSize(new GUIContent(levelText)).x,
                    style.CalcSize(new GUIContent(econDesc)).x
                ) + 40f,
                400f,
                Screen.width * 0.9f
            );

            DropShadowLabel(
                new Rect(centerX - labelWidth / 2f, topY, labelWidth, 40),
                econText,
                style
            );
            DropShadowLabel(
                new Rect(centerX - labelWidth / 2f, topY + 30, labelWidth, 35),
                rangeText,
                style
            );
            DropShadowLabel(
                new Rect(centerX - labelWidth / 2f, topY + 60, labelWidth, 35),
                levelText,
                style
            );
            DropShadowLabel(
                new Rect(centerX - labelWidth / 2f, topY + 90, labelWidth, 50),
                econDesc,
                style
            );
        }

        private void DropShadowLabel(Rect position, string content, GUIStyle style)
        {
            var originalColor = style.normal.textColor;

            style.normal.textColor = Color.black;
            GUI.Label(
                new Rect(position.x + 2, position.y + 2, position.width, position.height),
                content,
                style
            );

            style.normal.textColor = originalColor;
            GUI.Label(position, content, style);
        }
    }
}
