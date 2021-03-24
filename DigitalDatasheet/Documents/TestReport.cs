using DigitalDatasheet.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace DigitalDatasheet.Documents
{
    public class TestReport
    {
        Word.Application WordApp { get; set; }
        Word.Document WordDoc { get; set; }
        string WorkOrderNumber { get; set; }
        string TestCondition { get; set; }
        string TestConditionAbr { get; set; }
        string PartNumber { get; set; }
        string DateCode { get; set; }
        string TestPerformedOn { get; set; }
        private int DataRowCount { get; set; } = 0;
        private int DataTableIndex { get; set; } = 2;
        string DateReceivedYear { get; set; }
        readonly string ErrorLogFilePath = $@"\\ptlsrvr4\PTLOffice\Digital Datasheet Forms\Digital Datasheet Error Log\{DateTime.Now:D}.txt";
        //readonly string ErrorLogFilePath = $@"C:\Users\Nicholas\Documents\PTL\DigitalDatasheetErrorLog\{DateTime.Now:D}.txt";
        StreamWriter sw;

        public TestReport(string workOrderNumber, string testCondition, string partNumber, string dateCode, string testPerformedOn)
        {
            WorkOrderNumber = workOrderNumber;
            TestCondition = testCondition;
            TestConditionAbr = testCondition == "As Received" ? "AR" : "TS";
            PartNumber = partNumber;
            DateCode = dateCode;
            TestPerformedOn = testPerformedOn;

            try
            {
                WordApp = new Word.Application();
                //WordApp.Visible = true;
                //var test = Word.Documents
                WordDoc = WordApp.Documents.Open(@"\\ptlsrvr4\PTLOffice\Digital Datasheet Forms\report_template.docx", true, true);
                //WordDoc = WordApp.Documents.Open(@"C:\Users\Nicholas\Documents\PTL\report_template.docx", true, true);
                WordDoc.Activate();
                // make a copy of the report template before entering in any data to use in case number of data rows exceeds limit for one page
                WordApp.ActiveDocument.Bookmarks[@"\Page"].Range.Copy();
                // create correct page break type to add when needed
                //object breakType = Word.WdBreakType.wdPageBreak;
            }
            catch (Exception err)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport constructor -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
                sw.Close();

                if (WordDoc != null)
                    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                if (WordApp != null)
                    WordApp.Quit();
                throw;
            }
        }

        /// <summary>
        /// insert standard job information to test report (work order number, test condition, part number, date code)
        /// </summary>
        public void Set_Job_Info(string structureTitle = "")
        {
            try
            {
                WordApp.Selection.Find.Execute("<wo number>", true, true, false, false, false, true, 1, false, WorkOrderNumber, 2, false, false, false, false);
                WordApp.Selection.Find.Execute("<condition>", true, true, false, false, false, true, 1, false, TestCondition, 2, false, false, false, false);
                WordApp.Selection.Find.Execute("<part number>", true, true, false, false, false, true, 1, false, PartNumber, 2, false, false, false, false);
                WordApp.Selection.Find.Execute("<date code>", true, true, false, false, false, true, 1, false, DateCode, 2, false, false, false, false);
                WordApp.Selection.Find.Execute("<structure title>", true, true, false, false, false, true, 1, false, structureTitle, 2, false, false, false, false);
            }
            catch (Exception err)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport Set_Job_Info -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
                sw.Close();

                if (WordDoc != null)
                    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                if (WordApp != null)
                    WordApp.Quit();
                throw;
            }
        }
        public void Set_Single_Structure_Data_Row(string serialNumber, string location, List<string> dataRow, string acceptReject, bool distinctSn, int dataPerPage)
        {
            try
            {
                if (DataRowCount == dataPerPage)
                    Add_Page();
                Word.Table dataTable = WordDoc.Tables[DataTableIndex];
                if (!distinctSn)
                {
                    dataTable.Cell(2, 1).Range.Text += $"S/N {serialNumber} Loc {location}";
                    dataTable.Cell(5, 1).Range.Text += $"S/N {serialNumber} Loc {location}";
                }
                else
                {
                    dataTable.Cell(2, 1).Range.Text += serialNumber;
                    dataTable.Cell(5, 1).Range.Text += serialNumber;
                }
                int row = 2, column = 2;
                foreach (string value in dataRow)
                {
                    string printValue = string.Empty;
                    if (string.IsNullOrEmpty(value))
                        printValue = "###";
                    else
                    {
                        printValue = value;
                        if (value.Contains("^"))
                            printValue = value.Replace('^', ' ');
                        if (value.Contains("\n"))
                            printValue = value.Replace('\n', '/');
                    }
                    dataTable.Cell(row, column++).Range.Text += $"{printValue}";

                    if (column == 8 && row == 2)
                    {
                        row = 5;
                        column = 2;
                    }
                }
                if (acceptReject == "A")
                    dataTable.Cell(row, column).Range.Text += "Accept";
                else if (acceptReject == "R")
                    dataTable.Cell(row, column).Range.Text += "Non-Conformance";
                else if (!string.IsNullOrEmpty(acceptReject) && acceptReject.Contains("*"))
                    dataTable.Cell(row, column).Range.Text += "Customer-eval";
                else
                    dataTable.Cell(row, column).Range.Text += "###";

                //data_table.Cell(row, column).Range.Text += $"{accept_reject}";
                DataRowCount++;
            }
            catch (Exception ex)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport Set_Single_Structure_Data_Row -- {ex.Source}; {ex.TargetSite}\n{ex.Message}\n");
                sw.Close();

                if (WordDoc != null)
                    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                if (WordApp != null)
                    WordApp.Quit();
                throw;
            }
        }
        public void Set_Multiple_Structure_Data_Rows(string serialNumber, List<string> locations, List<string> structureTitles, List<List<string>> dataRows, List<string> acceptRejects, bool distinctSn, int dataPerPage)
        {
            try
            {
                if (DataRowCount == dataPerPage)
                    Add_Page();
                Word.Table dataTable = WordDoc.Tables[DataTableIndex];
                bool sameLocations = locations.Distinct().ToList().Count == 1;

                if (!distinctSn && sameLocations)
                {
                    dataTable.Cell(2, 1).Range.Text += $"S/N {serialNumber} Loc {locations[0]}";
                    dataTable.Cell(5, 1).Range.Text += $"S/N {serialNumber} Loc {locations[0]}";
                }
                else
                {
                    dataTable.Cell(2, 1).Range.Text += $"S/N: {serialNumber}";
                    dataTable.Cell(5, 1).Range.Text += $"S/N: {serialNumber}";
                }

                int row = 2, column = 2;
                string acceptReject = "";
                for (int i = 0; i < 13; i++)
                {
                    dataTable.Cell(row, column++).Range.Text += "";
                    if (column == 8 && row == 2)
                    {
                        row = 5;
                        column = 2;
                    }
                }
                for (int i = 0; i < structureTitles.Count; i++)
                {
                    if (!sameLocations && !distinctSn)
                    {
                        dataTable.Cell(2, 1).Range.Text += $"{structureTitles[i]} Loc {locations[i]}";
                        dataTable.Cell(5, 1).Range.Text += $"{structureTitles[i]} Loc {locations[i]}";
                    }
                    else
                    {
                        dataTable.Cell(2, 1).Range.Text += $"{structureTitles[i]}";
                        dataTable.Cell(5, 1).Range.Text += $"{structureTitles[i]}";
                    }

                    row = 2;
                    column = 2;

                    foreach (string value in dataRows[i])
                    {
                        string printValue = string.Empty;
                        if (string.IsNullOrEmpty(value))
                            printValue = "###";
                        else
                        {
                            printValue = value;
                            if (value.Contains("^"))
                                printValue = value.Replace('^', ' ');
                            if (value.Contains("\n"))
                                printValue = value.Replace('\n', '/');
                        }
                        dataTable.Cell(row, column++).Range.Text += $"{printValue}";

                        if (column == 8 && row == 2)
                        {
                            row = 5;
                            column = 2;
                        }
                    }
                    if (acceptRejects[i] == "A")
                        acceptReject = "Accept";
                    else if (acceptRejects[i] == "R")
                        acceptReject = "Non-Conformance";
                    else if (!string.IsNullOrEmpty(acceptReject) && acceptReject.Contains("*"))
                        acceptReject = "Customer-eval";
                    else
                        acceptReject = "###";

                    dataTable.Cell(row, column).Range.Text += $"{acceptReject}";
                }
                row = 2;
                column = 1;
                for (int i = 0; i < 15; i++)
                {
                    dataTable.Cell(row, column++).Range.Text += "";
                    if (column == 8 && row == 2)
                    {
                        row = 5;
                        column = 1;
                    }
                }
                DataRowCount += dataRows.Count;
            }
            catch (Exception ex)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport Set_Multiple_Structure_Data_Rows -- {ex.Source}; {ex.TargetSite}\n{ex.Message}\n");
                sw.Close();

                if (WordDoc != null)
                    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                if (WordApp != null)
                    WordApp.Quit();
                throw;
            }
        }
        public void Set_Internal_Layers(string internalLayers)
        {
            try
            {
                for (int i = 2; i <= WordDoc.Tables.Count; i += 2)
                {
                    if (string.IsNullOrEmpty(internalLayers)) return;
                    Word.Cell cell = WordDoc.Tables[i].Cell(2, 7);
                    string formattedInternalLayers = internalLayers.Remove(0, internalLayers.IndexOf(':') + 1);
                    string[] format_split = formattedInternalLayers.Split(';');
                    string newLayersMeasurement = "";
                    foreach (string layer in format_split)
                    {
                        string[] layerSections = layer.Trim().Split(':');
                        string layerNumbers = layerSections[0].Trim();
                        if (!layerNumbers.Contains("&"))
                        {
                            int lastCommaIndex = layerNumbers.LastIndexOf(",");
                            if (lastCommaIndex >= 0)
                                layerNumbers = layerNumbers.Remove(lastCommaIndex, 1).Insert(lastCommaIndex, " &");
                        }
                        string layerWeightMeasurement = layerSections[1].Trim();
                        layerWeightMeasurement = layerWeightMeasurement.Replace("oz", "oz.,");
                        newLayersMeasurement += $"\nLayers {layerNumbers}:\n{layerWeightMeasurement}\n";
                    }
                    if (newLayersMeasurement.ToLower().Contains("plus"))
                        newLayersMeasurement.Replace("plus", "+");
                    cell.Range.Text = newLayersMeasurement;
                    // underline top section of each set of layers
                    int wordCount = cell.Range.Words.Count;
                    //MessageBox.Show($"{word_count}");
                    for (int k = 1; k < wordCount; k++)
                    {
                        if (cell.Range.Words[k].Text.ToLower().StartsWith("layer"))
                        {
                            //MessageBox.Show($"{cell.Range.Words[k].Text}");
                            while (cell.Range.Words[k].Text != "\r" && k != wordCount)
                                cell.Range.Words[k++].Font.Underline = Word.WdUnderline.wdUnderlineSingle;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport Set_Internal_Layers -- {ex.Source}; {ex.TargetSite}\n{ex.Message}\n");
                sw.Close();

                if (WordDoc != null)
                    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                if (WordApp != null)
                    WordApp.Quit();
                throw;
            }
        }
        public void Set_Requirements(List<string> requirements)
        {
            try
            {
                // check how many data tables there are to determine how many times you must set the requirements
                for (int i = 2; i <= WordDoc.Tables.Count; i += 2)
                {
                    Word.Table dataTable = WordDoc.Tables[i];
                    int row = 3, column = 2, lastColumn = 8;
                    foreach (string requirement in requirements)
                    {
                        if (string.IsNullOrEmpty(requirement)) column++;
                        else
                        {
                            if (requirement.ToLower().StartsWith("layers"))
                            {
                                string reqText = requirement.Remove(0, requirement.IndexOf('\n') + 1);
                                string formattedReq = reqText;
                                if (reqText.Contains("\n"))
                                {
                                    // separate each set of equal thickness layers on individual lines
                                    string[] setLayers = reqText.Split('\n');
                                    formattedReq = $"Layers: {setLayers[0].Trim()}";
                                    for (int k = 1; k < setLayers.Length; k++)
                                    {
                                        setLayers[k] = setLayers[k].Trim();
                                        if (setLayers[k].ToLower().StartsWith("(stack") || setLayers[k].ToLower().StartsWith("stack")) continue;
                                        formattedReq += $"\nLayers: {setLayers[k]}";
                                    }
                                    formattedReq += "\n(Stack-up)";
                                    dataTable.Cell(row, column).Range.Text = formattedReq;
                                    column++;
                                }
                                else
                                    dataTable.Cell(row, column++).Range.Text = $"Layers: {reqText}";

                                //if (data_table.Cell(row-1, column).Range.Text.Contains("*"))
                                //    Set_Internal_Layers(data_table, formatted_req);
                            }
                            else
                                dataTable.Cell(row, column++).Range.Text = requirement;
                        }

                        if (column == lastColumn && row == 3)
                        {
                            row = 6;
                            column = 2;
                        }
                    }
                    // after each requirement is set, go through and split rows where needed
                    int tempRow = 6, tempColumn = 7, tempLastColumn = 1;
                    //foreach (string requirement in requirements)
                    for (int j = requirements.Count - 1; j >= 0; j--)
                    {
                        string reqText = requirements[j];
                        if (!string.IsNullOrEmpty(reqText))
                        {
                            if (reqText == "collapsed")
                            {
                                dataTable.Cell(tempRow, tempColumn).Range.Text = "";
                                dataTable.Cell(tempRow, tempColumn - 1).Merge(dataTable.Cell(tempRow, tempColumn));
                                //tempLastColumn--;
                                //tempColumn--;
                                //continue;
                            }
                            else if (reqText.Contains("\n") && 
                                !reqText.ToLower().StartsWith("layer") && 
                                !reqText.ToLower().StartsWith("x:") &&
                                !reqText.ToLower().StartsWith("negative") &&
                                !reqText.ToLower().StartsWith("smear"))
                            {
                                // clear text from requirement cell
                                dataTable.Cell(tempRow, tempColumn).Range.Text = "";
                                // separate each requirement
                                string[] reqSplit = reqText.Split('\n');
                                // split the current cell setting number of rows equal to numnber of requirements
                                dataTable.Cell(tempRow, tempColumn).Split(reqSplit.Length, 1);
                                int splitRow = tempRow;
                                foreach (string individual_req in reqSplit)
                                {
                                    string req = individual_req.Trim();
                                    dataTable.Cell(splitRow, tempColumn).Range.Text = req;
                                    dataTable.Cell(splitRow, tempColumn).Height = 0.1f;
                                    splitRow++;
                                }
                            }
                        }
                        tempColumn--;
                        if (tempColumn == tempLastColumn && tempRow == 6)
                        {
                            tempRow = 3;
                            tempColumn = 7;
                        }
                    }
                    row = 3;
                    column = 2;
                    lastColumn = 8;
                    for (int j = 0; j < 12; j++)
                    {
                        string reqText = dataTable.Cell(row, column).Range.Text;
                        if (string.IsNullOrEmpty(reqText)) continue;
                        if (reqText.Contains(" ("))
                        {
                            reqText = reqText.Replace(" (", "\n(");
                            int lastReturnIndex = reqText.LastIndexOf("\r");
                            if (lastReturnIndex != -1)
                                reqText = reqText.Remove(lastReturnIndex);
                            dataTable.Cell(row, column).Range.Text = reqText;
                        }
                        column++;
                        if (column == lastColumn && row == 3)
                        {
                            row = 6;
                            column = 2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport Set_Requirements -- {ex.Source}; {ex.TargetSite}\n{ex.Message}\n");
                sw.Close();

                if (WordDoc != null)
                    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                if (WordApp != null)
                    WordApp.Quit();
                throw;
            }
        }
        public void Underline_Serial_Number_Titles()
        {
            try
            {
                for (int i = 2; i <= WordDoc.Tables.Count; i += 2)
                {
                    var table = WordDoc.Tables[i];
                    int row = 2;
                    for (int j = 0; j < 2; j++)
                    {
                        Word.Cell cell = table.Cell(row, 1);
                        int wordCount = cell.Range.Words.Count;
                        for (int k = 1; k < wordCount; k++)
                        {
                            if (cell.Range.Words[k].Text == "S" && cell.Range.Words[k + 1].Text == "/")
                            {
                                while (cell.Range.Words[k].Text != "\r" && k != wordCount)
                                    cell.Range.Words[k++].Font.Underline = Word.WdUnderline.wdUnderlineSingle;
                            }
                        }
                        row = 5;
                    }
                }
            }
            catch (Exception err)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport Underline_Serial_Number_Titles -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
                sw.Close();

                if (WordDoc != null)
                    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                if (WordApp != null)
                    WordApp.Quit();
                throw;
            }
        }
        public void Set_Reject_Background_Color()
        {
            try
            {
                // set reject data background to yellow
                for (int i = 2; i <= WordDoc.Tables.Count; i += 2)
                {
                    var table = WordDoc.Tables[i];

                    int row = 2, column = 2;
                    for (int j = 0; j < 11; j++)
                    {
                        Word.Cell cell = table.Cell(row, column++);
                        int wordCount = cell.Range.Words.Count;
                        for (int k = 1; k < wordCount; k++)
                        {
                            Word.Range word = cell.Range.Words[k];
                            if (word.Text.StartsWith("R"))
                            {
                                word.Text = word.Text.Remove(0, 1);
                                while (cell.Range.Words[k].Text != "\r" && k != wordCount)
                                    cell.Range.Words[k++].Font.Shading.BackgroundPatternColor = Word.WdColor.wdColorYellow;
                            }
                        }
                        if (column == 8)
                        {
                            row = 5;
                            column = 2;
                        }
                    }
                }
                // set "non-conformance" status background to yellow in accept/reject column
                for (int i = 2; i <= WordDoc.Tables.Count; i += 2)
                {
                    var table = WordDoc.Tables[i];
                    Word.Cell cell = table.Cell(5, 8);

                    int wordCount = cell.Range.Words.Count;
                    for (int j = 1; j < wordCount; j++)
                    {
                        Word.Range word = cell.Range.Words[j];
                        if (word.Text == "Non" || word.Text == "-" || word.Text == "Conformance")
                            word.Font.Shading.BackgroundPatternColor = Word.WdColor.wdColorYellow;
                    }
                }
            }
            catch (Exception ex)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport Set_Reject_Background_Color -- {ex.Source}; {ex.TargetSite}\n{ex.Message}\n");
                sw.Close();

                if (WordDoc != null)
                    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                if (WordApp != null)
                    WordApp.Quit();
                throw;
            }
        }
        private void Add_Page()
        {
            // this part is added and run only when needed to copy and paste original template
            // first go to end of document
            WordApp.Selection.EndKey(Word.WdUnits.wdStory, Word.WdMovementType.wdMove);
            // insert page break
            WordApp.Selection.InsertBreak(Word.WdBreakType.wdPageBreak);
            // paste original page onto new page
            WordApp.Selection.PasteAndFormat(Word.WdRecoveryType.wdFormatOriginalFormatting);

            // set data_row_count and data_table_index to corect values
            DataRowCount = 0;
            DataTableIndex += 2;
        }
        public async Task Save_And_Close(string customer)
        {
            try
            {
                //app.Visible = true;
                // get path to directory of current work order number and determine if it exists
                // job path = \\ptlsrvr4 \ year received \ first letter of customer (i.e. _A to M) \ customer name \ work order number
                string letterFolder, fullDir, filePath;
                DateReceivedYear = await new AccessDb().GetJobYear(WorkOrderNumber);
                if (string.IsNullOrEmpty(DateReceivedYear))
                {
                    MessageBox.Show("Invalid date received for current job. Please check and make sure job information is correct.", "Job Date Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    WordApp.Visible = true;
                    return;
                }

                Regex letterFolderRegex = new Regex(@"^[a-mA-M0-9]");
                letterFolder = letterFolderRegex.IsMatch(customer) ? "_A to M" : "_N to Z";

                fullDir = $@"\\ptlsrvr4\j\{DateReceivedYear}\{letterFolder}\{customer}\{WorkOrderNumber}";
                //fullDir = $@"C:\Users\Nicholas\Documents\PTL";
                //DirectoryInfo job_path = new DirectoryInfo(full_dir);
                if (!Directory.Exists(fullDir))
                {
                    MessageBox.Show("The current job folder does not exist. Please check and make sure this work order number is correct and has been logged.", "No Job Folder", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    WordApp.Visible = true;
                    return;
                }
                filePath = $@"{fullDir}\{WorkOrderNumber}_{TestConditionAbr}_{TestPerformedOn}_Metallographic_Examination_test_report_data_only.docx";
                // determine if file already exists and ask before overwriting
                if (File.Exists(filePath))
                {
                    if (MessageBox.Show("Data only test report already exists. Saving current file will override existing test report. Would you like to continue and save anyway?", "File Already Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        WordApp.Visible = true;
                        return;
                    }
                }
                WordDoc.SaveAs2(filePath);
                if (MessageBox.Show("Data only test report has been created and saved in job folder. Would you like to open it now?", "Open Test Report", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        // open directoruy containing data report
                        Process.Start(fullDir);
                    }
                    catch (Exception) { }
                    // open data report
                    WordApp.Visible = true;
                }
                else
                {
                    WordDoc.Close();
                    WordApp.Quit();
                }
            }
            catch (Exception ex)
            {
                sw = new StreamWriter(ErrorLogFilePath, true);
                sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nReport Save_And_Close -- {ex.Source}; {ex.TargetSite}\n{ex.Message}\n");
                sw.Close();

                if (WordApp != null)
                    WordApp.Visible = true;
                //if (WordDoc != null)
                //    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                //if (WordApp != null)
                //    WordApp.Quit();
                throw;
            }
        }
        public void Close_Document()
        {
            // save updated document to correct work order number directory

            // setup options for closing original template document
            object saveOption = Word.WdSaveOptions.wdDoNotSaveChanges;
            object originalFormat = Word.WdOriginalFormat.wdOriginalDocumentFormat;
            object routeDocument = false;
            // close original template document without saving changes and quit the word app
            WordDoc.Close(saveOption, originalFormat, routeDocument);
            WordApp.Quit();
        }
    }
}