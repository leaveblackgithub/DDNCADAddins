using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;

namespace DDNCADAddins.Environments
{
    public class EditorHelper
    {
        private readonly Editor _acEditor;

        public EditorHelper(Editor acEditor)
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
            var message = "\n" + string.Join("\n", messageList);
            WriteMessage(message);
        }

        public void WriteMessage(Dictionary<string, List<dynamic>> messageDict)
        {
            var ge = messageDict.GetEnumerator();
            var messageList = new List<string>();
            while (ge.MoveNext()) messageList.Add(string.Join(":", ge.Current.Key, ge.Current.Value.Count.ToString()));
            ge.Dispose();
            WriteMessage(messageList);
        }

        public void WriteMessage(Dictionary<string, Dictionary<string, object>> messageDict)
        {
            var ge = messageDict.GetEnumerator();
            var messageList = new List<string>();
            while (ge.MoveNext())
            {
                var line = $"\n{ge.Current.Key}:\n";
                var ge2 = ge.Current.Value.GetEnumerator();
                while (ge2.MoveNext()) line += $"{ge2.Current.Key}:{ge2.Current.Value},";
                messageList.Add(line);
                ge2.Dispose();
            }

            ge.Dispose();
            WriteMessage(messageList);
        }

        public void Command(List<string> cmdList)

        {
            _acEditor.Command(cmdList.ToArray());
        }

        public void Regen()
        {
            _acEditor.Regen();
            _acEditor.UpdateScreen();
            Application.UpdateScreen();
            Utils.FlushGraphics();
        }

        public void SetByLayer()
        {
            _acEditor.Command("-setbylayer", "all", "", "y", "y");
        }

        public string GetString(string prompt)
        {
            var pso = new PromptStringOptions(prompt);
            var sres = _acEditor.GetString(pso);
            if (sres.Status == PromptStatus.OK) return sres.StringResult;
            return "";
        }

        public ObjectId GetEntity(string prompt, string rejectmsg = "", Type allowedclass = null)
        {
            var peo = new PromptEntityOptions(prompt);
            if (rejectmsg != "") peo.SetRejectMessage(rejectmsg);
            if (allowedclass != null) peo.AddAllowedClass(allowedclass, false);

            peo.AllowNone = true;
            var per = _acEditor.GetEntity(peo);

            if (per.Status != PromptStatus.OK) return ObjectId.Null;

            return per.ObjectId;
        }

        public SelectionSet GetSelection(string prompt)
        {
            WriteMessage(prompt);
            var pso = new PromptSelectionOptions();
            var per = _acEditor.GetSelection();

            if (per.Status != PromptStatus.OK) return null;

            var selectionSet = per.Value;
            WriteMessage($"\n{selectionSet.Count} objects selected.");
            return selectionSet;
        }

        public SelectionSet SelectAll(SelectionFilter filter)
        {
            var promptSelectionResult = _acEditor.SelectAll(filter);
            if (promptSelectionResult.Status == PromptStatus.OK) return promptSelectionResult.Value;
            return null;
        }
    }
}