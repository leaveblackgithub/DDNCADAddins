using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace CADAddins.Archive
{
    public class O_EditorHelper
    {
        private readonly Editor _acEditor;

        public O_EditorHelper(Editor acEditor)
        {
            _acEditor = acEditor;
        }

        public void WriteMessage(string message)
        {
            //            CadHelper.CurDocHelper.WriteMessage(message);
            //            System.Diagnostics.Trace.WriteLine(message);
            _acEditor.WriteMessage(message);
            _acEditor.UpdateScreen();
            Application.UpdateScreen();
        }

        public void WriteMessage(List<string> messageList)
        {
            string message = "\n" + string.Join("\n", messageList);
            WriteMessage(message);
        }

        public void WriteMessage(Dictionary<string,List<dynamic>> messageDict)
        {
            var ge = messageDict.GetEnumerator();
            List<string> messageList = new List<string>();
            while (ge.MoveNext())
            {
                messageList.Add(string.Join(":",ge.Current.Key,ge.Current.Value.Count.ToString()));
            }
            ge.Dispose();
            WriteMessage(messageList);
        }

        public void WriteMessage(Dictionary<string,Dictionary<string,object>> messageDict)
        {
            var ge = messageDict.GetEnumerator();
            List<string> messageList = new List<string>();
            while (ge.MoveNext())
            {
                string line = $"\n{ge.Current.Key}:\n";
                var ge2 = ge.Current.Value.GetEnumerator();
                while (ge2.MoveNext())
                {
                    line += $"{ge2.Current.Key}:{ge2.Current.Value.ToString()},";
                }
                messageList.Add(line);
                ge2.Dispose();
            }
            ge.Dispose();
            WriteMessage(messageList);
        }

        public dynamic SelectAll(SelectionFilter filter)
        {
            PromptSelectionResult promptSelectionResult = _acEditor.SelectAll(filter);
            if (promptSelectionResult.Status== PromptStatus.OK)return promptSelectionResult.Value.GetObjectIds();
            return null;
        }

        public void Command(List<string> cmdList)

        {
            _acEditor.Command(cmdList.ToArray());
        }

        public dynamic SelectNoneBlkOnLayer(string layerName)
        {
            TypedValue[] value = new TypedValue[6];
            value.SetValue(new TypedValue((int) DxfCode.Operator, "<and"), 0);
            value.SetValue(new TypedValue((int) DxfCode.LayerName, layerName), 1);
            value.SetValue(new TypedValue((int) DxfCode.Operator, "<not"), 2);
            value.SetValue(new TypedValue((int) DxfCode.Start, "INSERT"), 3);
            value.SetValue(new TypedValue((int) DxfCode.Operator, "not>"), 4);
            value.SetValue(new TypedValue((int) DxfCode.Operator, "and>"), 5);
            SelectionFilter filter = new SelectionFilter(value);
            dynamic ents = this.SelectAll(filter);
            return ents;
        }

        public dynamic SelectEntsOnLayer(string layerName)
        {
            TypedValue[] value = new TypedValue[1];
            value.SetValue(new TypedValue((int) DxfCode.LayerName, layerName), 0);
            SelectionFilter filter = new SelectionFilter(value);
            dynamic ents = this.SelectAll(filter);
            return ents;
        }

        public dynamic SelectEntsOnLayerOfLType(string layerName,string ltypeName)
        {
            TypedValue[] value = new TypedValue[4];
            value.SetValue(new TypedValue((int)DxfCode.Operator, "<and"), 0);
            value.SetValue(new TypedValue((int)DxfCode.LayerName, layerName), 1);
            value.SetValue(new TypedValue((int) DxfCode.LinetypeName, ltypeName), 2);
            value.SetValue(new TypedValue((int)DxfCode.Operator, "and>"), 3);
            SelectionFilter filter = new SelectionFilter(value);
            dynamic ents = this.SelectAll(filter);
            return ents;
        }

        public dynamic SelectEntsOnLayerOfLTypeNotByLayer(string layerName)
        {
            TypedValue[] value = new TypedValue[6];
            value.SetValue(new TypedValue((int)DxfCode.Operator, "<and"), 0);
            value.SetValue(new TypedValue((int)DxfCode.LayerName, layerName), 1);
            value.SetValue(new TypedValue((int)DxfCode.Operator, "<not"), 2);
            value.SetValue(new TypedValue((int)DxfCode.LinetypeName, "BYLAYER"), 3);
            value.SetValue(new TypedValue((int)DxfCode.Operator, "not>"), 4);
            value.SetValue(new TypedValue((int)DxfCode.Operator, "and>"), 5);
            SelectionFilter filter = new SelectionFilter(value);
            dynamic ents = this.SelectAll(filter);
            return ents;
        }

        public void Regen()
        {
            _acEditor.Regen();
            _acEditor.UpdateScreen();
            Application.UpdateScreen();
            Autodesk.AutoCAD.Internal.Utils.FlushGraphics();
        }

        public void SetByLayer()
        {
            _acEditor.Command("-setbylayer", "all", "","y", "y");
        }

        public dynamic SelectOfTextStyle(string oldName)
        {
            TypedValue[] value = new TypedValue[1];
            value.SetValue(new TypedValue((int)DxfCode.TextStyleName, oldName), 0);
            SelectionFilter filter = new SelectionFilter(value);
            dynamic ents = this.SelectAll(filter);
            return ents;
        }

        public string GetString(string prompt)
        {
            PromptStringOptions pso = new PromptStringOptions(prompt);
            PromptResult sres = _acEditor.GetString(pso);
            if (sres.Status != PromptStatus.OK) return sres.StringResult;
            return "";
        }

        public dynamic GetEntity(string prompt,string rejectmsg="",Type allowedclass=null)
        {
            PromptEntityOptions peo = new PromptEntityOptions(prompt);
            if (rejectmsg != "")
            {
                peo.SetRejectMessage(rejectmsg);
            }
            if (allowedclass!=null)
            { peo.AddAllowedClass(allowedclass, false);}

            peo.AllowNone = true;
            PromptEntityResult per = _acEditor.GetEntity(peo);

            if (per.Status != PromptStatus.OK) return ObjectId.Null;

            return per.ObjectId;
        }
        public SelectionSet GetSelection(string prompt)
        {
            WriteMessage(prompt);
            PromptSelectionOptions pso = new PromptSelectionOptions();
            PromptSelectionResult per = _acEditor.GetSelection();

            if (per.Status != PromptStatus.OK) return null;

            SelectionSet selectionSet = per.Value;
            WriteMessage($"\n{selectionSet.Count} objects selected.");
            return selectionSet;
        }

    }
}
