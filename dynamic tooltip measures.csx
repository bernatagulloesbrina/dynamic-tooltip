// '2021-05-11 / B.Agullo / 
// Creates the variables for dynamic tooltips


// Instructions: 
// If the main column chart x axis is 'Visual Config'[Category], 
// there should be a column "Tooltip Type" in the same table which specifies 
// which tooltip chart or element should shown for each axis Category
// 
// Select the column containing the tooltip type and click run, 
// or save this script as quick action

// by default for each tooltip type 4 mesures will be created
// Tooltip XXX Measure --> Values measure for the tooltip
// Tooltip XXX Title --> Title Measure for the tooltip 
// Tooltip XXX Background --> Background Measure for the tooltip 
// Tooltip XXX Data Color --> Conditional formatting Measure for the tooltip 


string valuesMeasureName = "Value";
string titleMeasureName = "Title";
string backgroundMeasureName = "Background";
string dataColorMeasureName = "Data Color"; 

string[] tooltipMeasures = {valuesMeasureName,titleMeasureName,backgroundMeasureName,dataColorMeasureName};
string tooltipMeasureSufix = "Tooltip";

string transparentColorMeasureName = "transparent";
 
string tooltipBackgroundColorMeasureName = "Tooltip Background Color";
string tooltipDefaultDataColorMeasureName = "Tooltip Default Data Color"; 
//string tooptipHighlightDataColorMeasureName = "Tooltip Highlight Data Color";

// -----


if (Selected.Columns.Count != 1) {
    Error("Select one and only one measure");
    return;
};

var tooltipTypeColumn = Selected.Column;
string tooltipTypeColumnReference = "'" + tooltipTypeColumn.Table.Name + "'[" + tooltipTypeColumn.Name + "]";

//check if [baground] and [transparent] measure already exist 
// and creates them if they don't exist 

if(!Model.AllMeasures.Any(Measure => Measure.Name == tooltipBackgroundColorMeasureName)) {
    tooltipTypeColumn.Table.AddMeasure(tooltipBackgroundColorMeasureName,"\"#FFFFFF\"");
};


if(!Model.AllMeasures.Any(Measure => Measure.Name == transparentColorMeasureName)) {
    tooltipTypeColumn.Table.AddMeasure(transparentColorMeasureName,"\"#FFFFFF00\"");
};


if(!Model.AllMeasures.Any(Measure => Measure.Name == tooltipDefaultDataColorMeasureName)) {
    tooltipTypeColumn.Table.AddMeasure(tooltipDefaultDataColorMeasureName,"\"#CCCCCC\"");
};

string query = "EVALUATE VALUES(" + tooltipTypeColumnReference + ")";
 
using (var reader = Model.Database.ExecuteReader(query))
{
    // Create a loop for every row in the resultset
    while(reader.Read())
    {
        string tooltipType = reader.GetValue(0).ToString();
        string measureDisplayFolder = tooltipType + " " + tooltipMeasureSufix + " Measures"; 
        
        foreach (string tooltipMeasure in tooltipMeasures) { 
        
            string measureName = tooltipType + " " + tooltipMeasureSufix + " " + tooltipMeasure ;
            string measureExpression = ""; 
            string measureDescription = tooltipMeasure + " measure for " + tooltipType + " " + tooltipMeasureSufix;
            
            if (tooltipMeasure == backgroundMeasureName) {
                measureExpression= 
                    "VAR Result =" + 
                     "    IF (" + 
                     "        SELECTEDVALUE ( " + tooltipTypeColumnReference + " ) = \"" + tooltipType + "\"," + 
                     "        [" + tooltipBackgroundColorMeasureName +"]," + 
                     "        [" + transparentColorMeasureName +"]" + 
                     "    )" + 
                     "RETURN" + 
                     "    FORMAT ( Result, \"@\" )";
                            
            }else if(tooltipMeasure == dataColorMeasureName) {
                measureExpression = 
                    "VAR Result =" + 
                     "    IF (" + 
                     "        SELECTEDVALUE ( " + tooltipTypeColumnReference + " ) = \"" + tooltipType + "\"," + 
                     "        [" + tooltipDefaultDataColorMeasureName + "] " + 
                     "    )" + 
                     "RETURN" + 
                     "    FORMAT ( Result, \"@\" )";
            }else if(tooltipMeasure == titleMeasureName) {
                measureExpression = 
                    "IF (" + 
                    "    SELECTEDVALUE ( " + tooltipTypeColumnReference + " ) = \"" + tooltipType + "\"," + 
                    "    /*replace \"" + tooltipType + "\" with your desired title expression*/" +
                    "    \"" + tooltipType + "\"" + 
                    ")";
           
           }else if(tooltipMeasure == valuesMeasureName) {
                 measureExpression = 
                    "IF (" + 
                    "    SELECTEDVALUE ( " + tooltipTypeColumnReference + " ) = \"" + tooltipType + "\"," + 
                    "    /*replace 1 with your desired measure*/" +
                    "    " + 1  + 
                    ")";
           
               
           };
             
           var newTooltipMeasure = tooltipTypeColumn.Table.AddMeasure(measureName, measureExpression);
           newTooltipMeasure.FormatDax(); 
           newTooltipMeasure.DisplayFolder = measureDisplayFolder; 
           newTooltipMeasure.Description = measureDescription; 
           
            
        };
    };
};   