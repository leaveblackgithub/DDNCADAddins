// (C) Copyright 2002-2014 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using System;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CADAddins.Reference;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

[assembly: CommandClass(typeof(EditorTest))]

//composed by Xiaodong Liang, Developer Technical Services, Autodesk

namespace CADAddins.Reference
{
    public class EditorTest
    {
        #region "send complete command"

        [CommandMethod("TestSimple")]
        public static void TestSimple()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //call Editor.Command to send command directly 
            ed.Command("Line", "0,0,0", "10,10,0", "20,50,0", "");
        }

        #endregion

        #region "send incomplete command by awaithe async"

        [CommandMethod("TestAwait")]
        public static async void TestAwait()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                // start Line command.  
                //after this line, the code hands over to AutoCAD, wait for user interaction
                await ed.CommandAsync("_.LINE", Editor.PauseToken, Editor.PauseToken);

                //After user completes the last interaction, this command TestAwait will be called again, but
                // begins from here: right after the last   await ed.CommandAsync
                // now we get the id of the first line
                {
                    var LastEnt = ed.SelectLast();
                    oIdArray.Add(LastEnt.Value[0].ObjectId);
                }


                // continue the next  interaction
                await ed.CommandAsync(Editor.PauseToken);


                //After user completes the last interaction, this command TestAwait will be called again, but
                // begins from here: right after the last   await ed.CommandAsync
                // now we get the id of the first line

                {
                    var LastEnt = ed.SelectLast();
                    oIdArray.Add(LastEnt.Value[0].ObjectId);
                }

                // end Line command
                await ed.CommandAsync("");

                // print id of each line
                foreach (ObjectId eachId in oIdArray)
                    ed.WriteMessage("line object ID: {0}\n", eachId.ToString());


                ed.WriteMessage("Done");
            }
            catch (Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }
        }

        #endregion

        #region "send incomplete command by callback function"

        //count of callback
        private static int _Count;

        //store the Id of each line
        private static readonly ObjectIdCollection oIdArray = new ObjectIdCollection();


        // declare the callback delegation
        private delegate void Del();

        private static Del _actionCompletedDelegate;

        // help function，check if Line command is running
        public static bool isLineActive()
        {
            var str = (string)Application.GetSystemVariable("CMDNAMES");
            if (str.Contains("LINE")) return true;
            return false;
        }

        // send incomplete command by callback function
        [CommandMethod("TestCallback")]
        public static void TestCallback()
        {
            _Count = 0;

            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            // start Line command, the first line 
            var cmdResult1 = ed.CommandAsync("_.LINE", Editor.PauseToken, Editor.PauseToken);

            // delegate callback function, wait for interaction ends 
            _actionCompletedDelegate = CreateLinesAsyncCallback;
            cmdResult1.OnCompleted(new Action(_actionCompletedDelegate));
        }

        // callback function 
        public static void CreateLinesAsyncCallback()
        {
            // AutoCAD hands over to the callback function 
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //if Line command is running 
            if (isLineActive())
            {
                if (_Count == 0)
                {
                    // get the first line ID 
                    var LastEnt = ed.SelectLast();
                    oIdArray.Add(LastEnt.Value[0].ObjectId);

                    // increase count 
                    _Count++;

                    // hand over to AutoCAD to execute the next interaction 
                    var cmdResult = ed.CommandAsync(Editor.PauseToken);
                    // delegate callback function, wait for interaction ends
                    _actionCompletedDelegate = CreateLinesAsyncCallback;
                    cmdResult.OnCompleted(new Action(_actionCompletedDelegate));
                }
                else if (_Count == 1)
                {
                    // get the second line ID 
                    var LastEnt = ed.SelectLast();
                    oIdArray.Add(LastEnt.Value[0].ObjectId);

                    // increase count 
                    _Count++;

                    // hand over to AutoCAD to execute the next interaction 
                    var cmdResult = ed.CommandAsync(Editor.PauseToken);
                    // delegate callback function, wait for interaction ends
                    _actionCompletedDelegate = CreateLinesAsyncCallback;
                    cmdResult.OnCompleted(new Action(_actionCompletedDelegate));
                }
            }
            else
            {
                //the end user ends the send command
                // display each id of the line
                foreach (ObjectId eachId in oIdArray)
                    ed.WriteMessage("\nline object ID: {0}", eachId.ToString());
            }
        }

        #endregion
    }
}