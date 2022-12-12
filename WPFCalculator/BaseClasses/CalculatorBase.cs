using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using WPFCalculator.Extensions;
using WPFCalculator.Helpers;
using WPFCalculator.Models;

namespace WPFCalculator.BaseClasses
{
    public class CalculatorBase : ComponentBase
    {
       
        protected bool HistoryTab { get; set; }
        protected bool MemoryTab { get; set; }
        protected bool NewEntry { get; set; }
        protected List<CalculatorHistory> HistoryList { get; set; }

        protected List<CalculatorHistory> TempHistoryList { get; set; }
        protected List<MemoryHistory> MemoryList { get; set; }

        protected CalculatorData calculatorData { get; set; }

        protected bool IsCalculatorView { get; set; } = true;
        protected bool IsImportXMLView { get; set; }
        protected bool IsHistoryGridView { get; set; }
        public CalculatorBase()
        {
            calculatorData = new CalculatorData();
            HistoryList = new List<CalculatorHistory>();
            MemoryList = new List<MemoryHistory>();
            SwitchTab(1); 
        }

       
        protected void ClearEntry()
        {
            calculatorData.CurrentEntryText = "0";
            calculatorData.RightValue = 0;
            LogHistory(HistoryList, "Clear Entry", "", 0);
        }
        protected void MathOperator_OnChanged(ChangeEventArgs args)
        {
            if (calculatorData.LeftValue == null && calculatorData.LastOperator == MathOperator.Default && calculatorData.NewOperator == MathOperator.Default)
            {
                LogHistory(HistoryList, "Enter", "", Convert.ToDouble(calculatorData.CurrentEntryText));
            }
            calculatorData.LeftValue = Convert.ToDouble(calculatorData.CurrentEntryText);
            calculatorData.HoldValue = calculatorData.LeftValue ?? 0;
            calculatorData.RightValue = null;
            var mathOperator = (MathOperator)Enum.Parse(typeof(MathOperator), args.Value.ToString());
            calculatorData.NewOperator = mathOperator;

        }
        protected void Clear()
        {
            calculatorData.CurrentEntryText = "0";
            calculatorData = new CalculatorData();
            LogHistory(HistoryList, "Clear", "", 0);
        }

        protected void ClearAll()
        {
            calculatorData.CurrentEntryText = "0";
            calculatorData = new CalculatorData();
            HistoryList = new List<CalculatorHistory>();
            MemoryList = new List<MemoryHistory>();
        }

        protected void ClearHistory()
        {
            HistoryList.Clear();
        }
        protected void RemoveLastKey()
        {
            if (calculatorData.RightValue != null)
            {
                if (calculatorData.CurrentEntryText.Length == 1)
                {
                    calculatorData.CurrentEntryText = "0";
                }
                else
                {
                    calculatorData.CurrentEntryText = calculatorData
                        .CurrentEntryText
                        .Remove(calculatorData.CurrentEntryText.Length - 1, 1);
                }
            }
        }

        protected void Append(string keyValue, bool isCopiedFromClipboard = false)
        {

            if (calculatorData.RightValue == null && !isCopiedFromClipboard)
            {
                calculatorData.CurrentEntryText = "0";
            }

            if (IsEntryValid(keyValue, calculatorData.CurrentEntryText))
            {
                if (calculatorData.CurrentEntryText == "0")
                {
                    if (keyValue == "0")
                    {
                        calculatorData.CurrentEntryText = "0";
                    }
                    else if (keyValue == ".")
                    {
                        calculatorData.CurrentEntryText = "0.";
                    }
                    else
                    {
                        calculatorData.CurrentEntryText = keyValue;
                    }
                }
                else if (calculatorData.CurrentEntryText != "0" && NewEntry)
                {
                    calculatorData.CurrentEntryText = keyValue;
                }
                else
                {
                    calculatorData.CurrentEntryText += keyValue;
                }
                NewEntry = false;
                calculatorData.RightValue = Convert.ToDouble(calculatorData.CurrentEntryText);

            }
        }

        protected void ChangeSign()
        {
            var toChangeSign = Convert.ToDouble(calculatorData.CurrentEntryText);
            if (toChangeSign != 0)
            {
                calculatorData.CurrentEntryText = (toChangeSign * -1).ToString();
            }

        }
        private static bool IsEntryValid(string value, string currentEntryText)
        {
            if (double.TryParse(value, out _))
            {
                return true;
            }
            if (value.Equals(".") && !currentEntryText.Contains('.'))
            {
                return true;
            }

            return false;
        }

        protected void Compute()
        {
            var result = (Double)0;
            var details = "";
            if (double.IsNaN(calculatorData.LeftValue ?? 0))
            {
                result = calculatorData.RightValue ?? 0;
                LogHistory(HistoryList, "Enter", "", result);
            }
            else if (calculatorData.NewOperator == MathOperator.Default && calculatorData.LastOperator != MathOperator.Default)
            {
                if (calculatorData.RightValue != null)
                {
                    LogHistory(HistoryList, "Enter", "", calculatorData.RightValue ?? 0);
                }
                result = ArithmeticOperationHelper.Compute(calculatorData.LastOperator.ToString(),
              calculatorData.RightValue ?? calculatorData.LeftValue ?? 0,
               calculatorData.HoldValue);
                details = $"{calculatorData.RightValue ?? calculatorData.LeftValue ?? 0} {calculatorData.LastOperator.GetDisplayName()} {calculatorData.HoldValue}";
                LogHistory(HistoryList, calculatorData.LastOperator.ToString(),
                   string.Empty,
                    calculatorData.HoldValue);
            }
            else if (calculatorData.NewOperator != MathOperator.Default)
            {
                result = ArithmeticOperationHelper.Compute(calculatorData.NewOperator.ToString(),
              calculatorData.LeftValue ?? 0, calculatorData.RightValue ?? calculatorData.HoldValue);
                calculatorData.HoldValue = calculatorData.RightValue ?? calculatorData.HoldValue;
                calculatorData.LastOperator = calculatorData.NewOperator;
                details = $"{calculatorData.LeftValue ?? 0} {calculatorData.LastOperator.GetDisplayName()} {calculatorData.RightValue ?? calculatorData.HoldValue}";
                LogHistory(HistoryList, calculatorData.NewOperator.ToString(),
                string.Empty,
                calculatorData.HoldValue);

            }
            else if (calculatorData.NewOperator == MathOperator.Default && calculatorData.LastOperator == MathOperator.Default)
            {

                result = calculatorData.RightValue ?? calculatorData.LeftValue ?? 0;
                var lastHistoryLog = HistoryList.LastOrDefault();

                details = result.ToString();
                if (lastHistoryLog == null)
                {
                    LogHistory(HistoryList, "Enter", "", result);
                }
                else if (lastHistoryLog.Hist_Action == "Equal" && result != lastHistoryLog.Hist_Value)
                {
                    LogHistory(HistoryList, "Enter", "", result);
                }


            }
            LogHistory(HistoryList, "Equal", $"{details} =", result);
            calculatorData.LeftValue = result;
            calculatorData.CurrentEntryText = result.ToString();
            calculatorData.RightValue = null;
            calculatorData.NewOperator = MathOperator.Default;
        }

        protected void SwitchTab(int tabNum)
        {
            switch (tabNum)
            {
                case 1:
                    HistoryTab = true;
                    MemoryTab = false;
                    break;
                case 2:
                    HistoryTab = false;
                    MemoryTab = true;
                    break;
            }
        }

        private static void LogHistory(List<CalculatorHistory> history, string operation, string details, double value)
        {
            history.Add(
            new CalculatorHistory
            {
                Hist_ID = history.Count + 1,
                Hist_Action = operation,
                Hist_Details = details,
                Hist_Value = value
            });
        }

        protected void RecallMemory(List<MemoryHistory> memoryHistoryList)
        {
            if (memoryHistoryList.Any())
            {
                var lastMemoryAdded = memoryHistoryList.Last();
                calculatorData.CurrentEntryText = lastMemoryAdded.Value.ToString();
                calculatorData.RightValue = lastMemoryAdded.Value;

            }
        }

        protected void UpdateLastMemoryValue(int symbol)
        {
            var lastMemory = MemoryList.LastOrDefault();
            if (lastMemory != null)
            {
                lastMemory.Value += ((calculatorData.RightValue ?? 0) * symbol);
                NewEntry = true;
            }
        }

        protected void StoreMemory()
        {
            MemoryList.Add(new MemoryHistory
            {
                Id = MemoryList.Count + 1,
                Value = calculatorData.RightValue ?? calculatorData.LeftValue ?? 0
            });
            NewEntry = true;
        }

        protected static void ExportXML(object obj, string fileName)
        {
            UtilityHelper.DownloadXMLFile(obj, fileName);

        }

        protected void ViewImportXMLScreen()
        {
            IsCalculatorView = false;
            IsHistoryGridView = false;
            IsImportXMLView = true;
        }

        protected void ViewCalculatorScreen()
        {
            IsCalculatorView = true;
            IsHistoryGridView = false;
            IsImportXMLView = false;
        }
        protected void UploadImportedXML()
        {
            IsCalculatorView = true;
            IsHistoryGridView = false;
            IsImportXMLView = false;

            HistoryList.AddRange(TempHistoryList);
            var lastHistoryLine = HistoryList.LastOrDefault();
            calculatorData.CurrentEntryText = lastHistoryLine.Hist_Value.ToString();
            calculatorData.LeftValue = lastHistoryLine.Hist_Value;
            calculatorData.NewOperator = lastHistoryLine.Hist_Action == "Equal" ?
                MathOperator.Default :
                (MathOperator)Enum.Parse(typeof(MathOperator), lastHistoryLine.Hist_Action);
            calculatorData.LastOperator = calculatorData.NewOperator;
            TempHistoryList.Clear();
            MessageBox.Show($"XML successfully imported.");
            return;
        }

        protected void ViewHistoryGridScreen()
        {
            IsCalculatorView = false;
            IsHistoryGridView = true;
            IsImportXMLView = false;
        }
        protected async Task ImportHistoryXML(InputFileChangeEventArgs e)
        {
            if (e.File.ContentType == "text/xml")
            {
                Stream stream = e.File.OpenReadStream();
                MemoryStream memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);
                stream.Close();
                var bytes = memoryStream.ToArray();
                memoryStream = new MemoryStream(bytes.ToArray());
                using (var textReader = new StreamReader(memoryStream))
                {
                    string xml = textReader.ReadToEnd();
                    TempHistoryList = UtilityHelper.DeserializeXmlString<List<CalculatorHistory>>(xml);
                }
             
            }

        }

        protected void GetClipboardEntry()
        {
            if (IsCalculatorView)
            {
                var entry = UtilityHelper.GetClipboardValue();
                if (!string.IsNullOrWhiteSpace(entry))
                {
                    Append(entry, true);
                }
            }

        }


    }


}

