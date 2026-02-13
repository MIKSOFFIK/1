using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

public class Game : MonoBehaviour
{
    [SerializeField] private double score;
    [SerializeField] private double money;
    private int ClickValue = 1;
    public bool Nazar_is_true = false;

    public Text scoreText, moneyText;
    public double tankLimit = 100;
    public GameObject Shop,SellPan,SkinPan,PromoPan;
    public GameObject RareSkinPan,EpicSkinPan,LegaSkinPan,SecretSkinPan;
    public GameObject Skin0,Skin1,Skin2,Skin3,Skin4,Skin5,NAZAR;
    public GameObject NAZAR_Button;
    public string save_file_path = "save.json";
    
    // Массивы для магазина (нужно проинициализировать в инспекторе)
    public int[] LvlCost;
    public Text[] LvlText;
    public int[] TankCost;
    public Text[] TankText;
    public int[] SellUpCost;
    public float[] SellCost;
    public Text[] SellCostText, sellText;
    
    // Добавляем CultureInfo для корректного парсинга чисел
    private CultureInfo cultureInfo = CultureInfo.InvariantCulture;

    private void Start(){
        Load_progpess();
        UpdateScoreText();
        UpdateMoneyText();
    }
    public void Save_progpess(){
        // ИСПРАВЛЕНО: используем SetString вместо GetString
        PlayerPrefs.SetString("score", score.ToString(cultureInfo));
        PlayerPrefs.SetString("tankLimit", tankLimit.ToString(cultureInfo));
        PlayerPrefs.SetString("money", money.ToString(cultureInfo));
        PlayerPrefs.SetInt("ClickValue", ClickValue);
        
        // Сохраняем массивы
        SaveArray("LvlCost", LvlCost);
        SaveArray("TankCost", TankCost);
        SaveArray("SellUpCost", SellUpCost);
        SaveFloatArray("SellCost", SellCost);
        
        PlayerPrefs.Save(); 
        Debug.Log($"Данные сохранены: score={score}, money={money}, tankLimit={tankLimit}");
    }
    
    public void Load_progpess(){
        try
        {
            // Безопасный парсинг с обработкой исключений
            score = SafeParseDouble(PlayerPrefs.GetString("score", "0"));
            tankLimit = SafeParseDouble(PlayerPrefs.GetString("tankLimit", "100"));
            money = SafeParseDouble(PlayerPrefs.GetString("money", "0.0"));
            ClickValue = PlayerPrefs.GetInt("ClickValue", 1);
            
            // Загружаем массивы
            LvlCost = LoadArray("LvlCost", LvlCost);
            TankCost = LoadArray("TankCost", TankCost);
            SellUpCost = LoadArray("SellUpCost", SellUpCost);
            SellCost = LoadFloatArray("SellCost", SellCost);
            
            Debug.Log($"Данные загружены: score={score}, money={money}, tankLimit={tankLimit}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка загрузки: {e.Message}");
            ResetToDefault();
        }
        
        // Обновляем UI после загрузки
        UpdateUIAfterLoad();
    }
    
    private double SafeParseDouble(string value){
        if (string.IsNullOrEmpty(value))
            return 0.0;
            
        try{
            // Используем InvariantCulture для корректного парсинга
            return double.Parse(value, CultureInfo.InvariantCulture);
        }
        catch{
            // Пробуем заменить запятую на точку
            value = value.Replace(',', '.');
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;
            return 0.0;
        }
    }
    
    private void SaveArray(string key, int[] array){
        if (array == null) return;
        
        for (int i = 0; i < array.Length; i++){
            PlayerPrefs.SetInt($"{key}_{i}", array[i]);
        }
        PlayerPrefs.SetInt($"{key}_length", array.Length);
    }
    
    private int[] LoadArray(string key, int[] defaultArray){
        if (!PlayerPrefs.HasKey($"{key}_length")){
            return defaultArray;
        }
        
        int length = PlayerPrefs.GetInt($"{key}_length");
        int[] array = new int[length];
        
        for (int i = 0; i < length; i++){
            array[i] = PlayerPrefs.GetInt($"{key}_{i}", defaultArray != null && i < defaultArray.Length ? defaultArray[i] : 0);
        }
        
        return array;
    }
    
    private void SaveFloatArray(string key, float[] array){
        if (array == null) return;
        
        for (int i = 0; i < array.Length; i++){
            PlayerPrefs.SetFloat($"{key}_{i}", array[i]);
        }
        PlayerPrefs.SetInt($"{key}_length", array.Length);
    }
    
    private float[] LoadFloatArray(string key, float[] defaultArray){
        if (!PlayerPrefs.HasKey($"{key}_length")){
            return defaultArray;
        }
        
        int length = PlayerPrefs.GetInt($"{key}_length");
        float[] array = new float[length];
        
        for (int i = 0; i < length; i++){
            array[i] = PlayerPrefs.GetFloat($"{key}_{i}", defaultArray != null && i < defaultArray.Length ? defaultArray[i] : 0f);
        }
        
        return array;
    }
    
    private void ResetToDefault(){
        score = 0;
        money = 0.0;
        tankLimit = 100;
        ClickValue = 1;
        Debug.Log("Сброс к значениям по умолчанию");
    }
    
    private void UpdateUIAfterLoad(){
        // Обновляем тексты цен в UI
        if (LvlText != null && LvlText.Length > 0 && LvlCost != null && LvlCost.Length > 0){
            LvlText[0].text = FormatNumber(LvlCost[0]);
        }
        
        if (TankText != null && TankText.Length > 0 && TankCost != null && TankCost.Length > 0){
            TankText[0].text = FormatNumber(TankCost[0]);
        }
        
        if (SellCostText != null && SellCostText.Length > 0 && SellUpCost != null && SellUpCost.Length > 0){
            SellCostText[0].text = FormatNumber(SellUpCost[0]);
        }
        
        if (sellText != null && sellText.Length > 0 && SellCost != null && SellCost.Length > 0){
            sellText[0].text = FormatNumber(SellCost[0], true);
        }
        
        UpdateScoreText();
        UpdateMoneyText();
    }
    
    public void OnClickButton()
    {
        score += ClickValue;
        if (score > tankLimit)
        {
            score = tankLimit;
        }
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = FormatNumber(score);
    }

    private void UpdateMoneyText()
    {
        moneyText.text = "₽" + FormatNumber(money, true);
    }
    
    // Функция форматирования чисел (ДОБАВЬТЕ ЭТОТ МЕТОД)
    private string FormatNumber(double num, bool showDecimals = false)
    {
        // Для денег показываем 2 знака после запятой для маленьких значений
        if (showDecimals && num < 1000)
        {
            return num.ToString("F2", cultureInfo);
        }

        // Для обычных чисел без десятичных знаков при малых значениях
        if (!showDecimals && num < 1000)
        {
            return num.ToString("F0", cultureInfo);
        }

        // Форматирование с суффиксами
        string[] suffixes = { "", "k", "M", "B", "T", "Q" };
        int suffixIndex = 0;
        double formattedNum = num;

        while (formattedNum >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            formattedNum /= 1000;
            suffixIndex++;
        }

        // Определяем количество знаков после запятой
        string formatString;
        if (showDecimals)
        {
            // Для денег: 1 знак после запятой для больших чисел
            formatString = formattedNum < 10 ? "F2" : "F1";
        }
        else
        {
            // Для счета: 1 знак после запятой, если не целое число
            formatString = formattedNum % 1 == 0 ? "F0" : "F1";
        }

        return formattedNum.ToString(formatString, cultureInfo) + suffixes[suffixIndex];
    }
    
    // Методы магазина (как у вас были)
    public void OnClickBuyLvl()
    {
        if (money >= LvlCost[0])
        {
            money -= LvlCost[0];
            LvlCost[0] = Mathf.RoundToInt(LvlCost[0] * 1.7f);
            ClickValue = Mathf.RoundToInt(ClickValue * 1.5f);
            LvlText[0].text = FormatNumber(LvlCost[0]);
            UpdateMoneyText();
            Save_progpess(); // Сохраняем после покупки
        }
    }

    public void OnClickBuyTank()
    {
        if (money >= TankCost[0])
        {
            money -= TankCost[0];
            TankCost[0] *= 3;
            tankLimit *= 2;
            TankText[0].text = FormatNumber(TankCost[0]);
            UpdateMoneyText();
            Save_progpess(); // Сохраняем после покупки
        }
    }

    public void OnClickBuySellCost()
    {
        if (money >= SellUpCost[0])
        {
            money -= SellUpCost[0];
            SellUpCost[0] *= 5;
            SellCost[0] += 0.01f;
            SellCostText[0].text = FormatNumber(SellUpCost[0]);
            sellText[0].text =  FormatNumber(SellCost[0], true);
            UpdateMoneyText();
            Save_progpess(); // Сохраняем после покупки
        }
    }

    public void OnClickSell()
    {
        if (score > 0)
        {
            double moneyToAdd = Math.Round(score * SellCost[0], 2);
            money += moneyToAdd;
            score = 0;

            UpdateScoreText();
            UpdateMoneyText();
            Save_progpess(); // Сохраняем после продажи
        }
    }
    private void HideAllPan()
    {
        SellPan.SetActive(false);
        Shop.SetActive(false);
        SkinPan.SetActive(false);
        PromoPan.SetActive(false);   
    }

    public void ShowAndHideShop()
    {
            if (SellPan != null && SellPan.activeSelf)
            {
                SellPan.SetActive(false);
            }
            if (SkinPan != null && SkinPan.activeSelf)
            {
                SkinPan.SetActive(false);
            }
            if (PromoPan != null && PromoPan.activeSelf)
            {
                PromoPan.SetActive(false);
            }
            Shop.SetActive(!Shop.activeSelf);
    }

    public void ShowAndHideSell()
    {
        if (SellPan != null)
        {
            // Сначала скрываем Shop, если он активен
            if (Shop != null && Shop.activeSelf)
            {
                Shop.SetActive(false);
            }
            if (SkinPan != null && SkinPan.activeSelf)
            {
                SkinPan.SetActive(false);
            }
            if (PromoPan != null && PromoPan.activeSelf)
            {
                PromoPan.SetActive(false);
            }

            // Затем переключаем состояние SellPan
            SellPan.SetActive(!SellPan.activeSelf);
        }
    }
    public void ShowAndHideSkin()
    {
        if(SkinPan != null)
        {
            if(Shop != null && Shop.activeSelf)
            {
                Shop.SetActive(false);
            }
             if (SellPan != null && SellPan.activeSelf)
            {
                SellPan.SetActive(false);
            }
            if (PromoPan != null && PromoPan.activeSelf)
            {
                PromoPan.SetActive(false);
            }

            SkinPan.SetActive(!SkinPan.activeSelf);
        }
    }
    public void ShowAndHidePromoPan()
    {
         if(Shop != null && Shop.activeSelf)
        {
            Shop.SetActive(false);
        }
        if (SellPan != null && SellPan.activeSelf)
        {
            SellPan.SetActive(false);
        }
        if (SkinPan != null && SkinPan.activeSelf)
        {
            SkinPan.SetActive(false);
        }
        PromoPan.SetActive(!PromoPan.activeSelf);
        
    }
    public void ShowAndHideRareSkin(){if(RareSkinPan != null){RareSkinPan.SetActive(!RareSkinPan.activeSelf);}}
    public void ShowAndHideEpicSkin(){if(EpicSkinPan != null){EpicSkinPan.SetActive(!EpicSkinPan.activeSelf);}}
    public void ShowAndHideLegaSkin(){if(LegaSkinPan != null){LegaSkinPan.SetActive(!LegaSkinPan.activeSelf);}}
    public void ShowAndHideSecretSkin(){if(SecretSkinPan != null){SecretSkinPan.SetActive(!SecretSkinPan.activeSelf);}}
    
    //скины 
    private void CloseSkin()
    {
        Skin0.SetActive(false);
        Skin1.SetActive(false);
        Skin2.SetActive(false);
        Skin3.SetActive(false);
        Skin4.SetActive(false);
        Skin5.SetActive(false);
    }

    public void ChooseSkin0()
    {
        CloseSkin(); // Добавлены скобки ()
        Skin0.SetActive(true);
    }

    public void ChooseSkin1()
    {
        CloseSkin(); // Добавлены скобки ()
        Skin1.SetActive(true);
    }

    public void ChooseSkin2()
    {
        CloseSkin(); // Добавлены скобки ()
        Skin2.SetActive(true);
    }

    public void ChooseSkin3()
    {
        CloseSkin(); // Добавлены скобки ()
        Skin3.SetActive(true);
    }

    public void ChooseSkin4()
    {
        CloseSkin(); // Добавлены скобки ()
        Skin4.SetActive(true);
    }
    public void ChooseSkin5()
    {
        CloseSkin(); // Добавлены скобки ()
        Skin5.SetActive(true);
    }
    public void ChooseSkinNAZAR()
    {
        CloseSkin(); // Добавлены скобки ()
        NAZAR.SetActive(true);
    }
    
    public InputField inputField;
    public void PromoCodes()
    {
        string code = inputField.text;
        if(code == "НазарАссенизатор")
        {
            Nazar_is_true = true;
            if(Nazar_is_true)
            {
            NAZAR_Button.SetActive(true);
            }
        }
        
    }
   
    
}