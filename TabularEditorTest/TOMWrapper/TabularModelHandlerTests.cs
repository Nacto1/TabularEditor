﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Tests
{
    public class MockTree : TabularTree
    {
        public MockTree(Model model) : base(model)
        {
        }

        public override void OnNodesChanged(ITabularObject nodeItem)
        {
        }

        public override void OnNodesInserted(ITabularObject parent, params ITabularObject[] children)
        {
        }

        public override void OnNodesRemoved(ITabularObject parent, params ITabularObject[] children)
        {
        }

        public override void OnStructureChanged(ITabularNamedObject obj = null)
        {
        }
    }

    [TestClass()]
    public class TabularModelHandlerTests
    {
        [TestMethod()]
        public void ConnectTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);

            Assert.AreEqual("AdventureWorks", handler.Model.Database.Name);
        }

        [TestMethod()]
        public void DuplicateTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);

            var c = handler.Model.Tables["Date"].AddCalculatedColumn();
            c.Clone("Cloned Column");

            var m = handler.Model.Tables["Date"].AddMeasure();
            m.Clone("Cloned Measure");

            handler.SaveDB();
            Assert.IsTrue(handler.Model.Tables["Date"].Columns.Contains("Cloned Column"));
            Assert.IsTrue(handler.Model.Tables["Date"].Measures.Contains("Cloned Measure"));

            handler.UndoManager.Rollback();
            handler.SaveDB();
        }

        [TestMethod()]
        public void AddMeasureTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            handler.Model.Tables["Internet Sales"].AddMeasure("TestMeasure", "SUM('Internet Sales'[Sales Amount])", "Test folder");
            handler.SaveDB();

            Assert.AreEqual("", handler.Model.Tables["Internet Sales"].Measures["TestMeasure"].ErrorMessage);

            // Rollback:
            handler.UndoManager.Rollback();
            handler.SaveDB();
            Assert.IsFalse(handler.Model.Tables["Internet Sales"].Measures.Contains("TestMeasure"));
        }

        [TestMethod()]
        public void DeleteTableTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            handler.Model.Tables["Currency"].Delete();
            handler.SaveDB();

            handler.UndoManager.Rollback();

            handler.SaveDB();
            Assert.IsTrue(handler.Model.Tables.Contains("Currency"));
        }

        [TestMethod()]
        public void DeleteLevelTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            handler.Model.Tables["Date"].Hierarchies["Calendar"].Levels["Quarter"].Delete();
            handler.SaveDB();

            var h = handler.Model.Tables["Date"].Hierarchies["Calendar"];
            Assert.AreEqual(0, h.Levels["Year"].Ordinal);
            Assert.AreEqual(1, h.Levels["Semester"].Ordinal);
            Assert.AreEqual(2, h.Levels["Month"].Ordinal);
            Assert.AreEqual(3, h.Levels["Day"].Ordinal);

            // Rollback:
            handler.UndoManager.Rollback();
            handler.SaveDB();
            Assert.AreEqual(0, h.Levels["Year"].Ordinal);
            Assert.AreEqual(1, h.Levels["Semester"].Ordinal);
            Assert.AreEqual(2, h.Levels["Quarter"].Ordinal);
            Assert.AreEqual(3, h.Levels["Month"].Ordinal);
            Assert.AreEqual(4, h.Levels["Day"].Ordinal);
        }

        [TestMethod()]
        public void DeleteMeasureTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            handler.Model.Tables["Internet Sales"].Measures["Internet Total Sales"].Delete();
            handler.SaveDB();

            Assert.IsFalse(handler.Model.Tables["Internet Sales"].Measures.Contains("Internet Total Sales"));

            // Rollback:
            handler.UndoManager.Rollback();
            handler.SaveDB();
            Assert.IsTrue(handler.Model.Tables["Internet Sales"].Measures.Contains("Internet Total Sales"));
        }

        [TestMethod()]
        public void RenameHierarchyTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            handler.Model.Tables["Date"].Hierarchies["Calendar"].Name = "Calendar Renamed";
            handler.SaveDB();

            Assert.IsTrue(handler.Model.Tables["Date"].Hierarchies.Contains("Calendar Renamed"));

            handler.UndoManager.Rollback();
            handler.SaveDB();
            Assert.IsFalse(handler.Model.Tables["Date"].Hierarchies.Contains("Calendar Renamed"));
        }

        [TestMethod()]
        public void DeleteHierarchyTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            handler.Model.Tables["Date"].Hierarchies["Calendar"].Delete();
            handler.SaveDB();

            Assert.IsFalse(handler.Model.Tables["Date"].Hierarchies.Contains("Calendar"));

            handler.UndoManager.Rollback();
            handler.SaveDB();
            Assert.IsTrue(handler.Model.Tables["Date"].Hierarchies.Contains("Calendar"));
        }

        [TestMethod()]
        public void DeleteHierarchyAndLevelTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            var hierarchy = handler.Model.Tables["Date"].Hierarchies["Calendar"];
            hierarchy.Delete();
            handler.SaveDB();

            Assert.IsFalse(handler.Model.Tables["Date"].Hierarchies.Contains("Calendar"));

            handler.UndoManager.Rollback();
            handler.SaveDB();
            Assert.IsTrue(handler.Model.Tables["Date"].Hierarchies.Contains("Calendar"));

            hierarchy.Levels["Quarter"].Delete();
            hierarchy.Levels["Day"].Delete();
            hierarchy.Levels["Year"].Delete();
            handler.SaveDB();
            Assert.AreEqual(0, hierarchy.Levels["Semester"].Ordinal);
            Assert.AreEqual(1, hierarchy.Levels["Month"].Ordinal);
            hierarchy.Delete();
            handler.SaveDB();

            handler.UndoManager.Rollback();
            handler.SaveDB();

            Assert.AreEqual(0, hierarchy.Levels["Year"].Ordinal);
            Assert.AreEqual(1, hierarchy.Levels["Semester"].Ordinal);
            Assert.AreEqual(2, hierarchy.Levels["Quarter"].Ordinal);
            Assert.AreEqual(3, hierarchy.Levels["Month"].Ordinal);
            Assert.AreEqual(4, hierarchy.Levels["Day"].Ordinal);
        }

        [TestMethod()]
        public void CreateHierarchyTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            var table = handler.Model.Tables["Employee"];
            table.AddHierarchy("Test Hierarchy", null, table.Columns["Base Rate"], table.Columns["Birth Date"], table.Columns["Department Name"]);
            handler.SaveDB();

            Assert.AreEqual(3, table.Hierarchies["Test Hierarchy"].Levels.Count);

            handler.UndoManager.Rollback();
            handler.SaveDB();
            Assert.IsFalse(table.Hierarchies.Contains("Test Hierarchy"));
        }

        public void ResetPerspectives()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            var p = handler.Model.Perspectives;
            if (p.Contains("Test")) p["Test"].Delete();
            if (p.Contains("Testx")) p["Testx"].Delete();
            if (p.Contains("Test Perspective Inclusive")) p["Test Perspective Inclusive"].Delete();
            if (p.Contains("Test Perspective Exclusive")) p["Test Perspective Exclusive"].Delete();
            handler.SaveDB();

        }

        /// <summary>
        /// Comprehensive test of Perspective functionality.
        /// Adds two new perspectives to the model: "Test Perspective Inclusive" and "Test Perspective Exclusive".
        /// The former includes only a few select items, through table assignment, object collection assignment
        /// and direct assignment. The latter includes everything (through table collection assignment), but then
        /// excludes a few select items.
        /// </summary>
        [TestMethod()]
        public void PerspectiveTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);

            var m = handler.Model;

            // Perspective which only includes a few select items:
            var pnIn = "Test Perspective Inclusive";
            m.Perspectives.Add(new Perspective() { Name = pnIn });
            m.Tables["Employee"].InPerspective[pnIn] = true;
            m.Tables["Date"].Hierarchies.InPerspective(pnIn, true);
            m.Tables["Internet Sales"].Measures.InPerspective(pnIn, true);
            m.Tables["Product"].Columns.InPerspective(pnIn, true);
            m.Tables["Reseller Sales"].Measures["Reseller Total Sales"].InPerspective[pnIn] = true;

            // Perspective which includes everything except a few select items:
            var pnEx = "Test Perspective Exclusive";
            m.Perspectives.Add(new Perspective() { Name = pnEx });
            m.Tables.InPerspective(pnEx, true);
            m.Tables["Employee"].InPerspective[pnEx] = false;
            m.Tables["Date"].Hierarchies.InPerspective(pnEx, false);
            m.Tables["Internet Sales"].Measures.InPerspective(pnEx, false);
            m.Tables["Product"].Columns.InPerspective(pnEx, false);
            m.Tables["Reseller Sales"].Measures["Reseller Total Sales"].InPerspective[pnEx] = false;

            handler.SaveDB();

            // Check on included perspective:
            Assert.IsTrue(m.Tables["Employee"].GetChildren().OfType<ITabularPerspectiveObject>().All(obj => obj.InPerspective[pnIn]));
            Assert.IsTrue(m.Tables["Employee"].InPerspective[pnIn]);
            Assert.IsTrue(m.Tables["Date"].Hierarchies.All(obj => obj.InPerspective[pnIn]));
            Assert.IsTrue(m.Tables["Internet Sales"].Measures.All(obj => obj.InPerspective[pnIn]));
            Assert.IsTrue(m.Tables["Product"].Columns.All(obj => obj.InPerspective[pnIn]));
            Assert.IsTrue(m.Tables["Reseller Sales"].Measures["Reseller Total Sales"].InPerspective[pnIn]);
            Assert.AreEqual(1, m.Tables["Reseller Sales"].Measures.Count(obj => obj.InPerspective[pnIn]));
            Assert.IsTrue(m.Tables["Reseller Sales"].InPerspective[pnIn]);

            Assert.IsFalse(m.Tables["Geography"].GetChildren().OfType<ITabularPerspectiveObject>().Any(obj => obj.InPerspective[pnIn]));
            Assert.IsFalse(m.Tables["Geography"].InPerspective[pnIn]);
            Assert.IsFalse(m.Tables["Date"].Columns.Any(obj => obj.InPerspective[pnIn]));

            // Check on excluded perspective:
            Assert.IsFalse(m.Tables["Employee"].GetChildren().OfType<ITabularPerspectiveObject>().Any(obj => obj.InPerspective[pnEx]));
            Assert.IsFalse(m.Tables["Employee"].InPerspective[pnEx]);
            Assert.IsFalse(m.Tables["Date"].Hierarchies.Any(obj => obj.InPerspective[pnEx]));
            Assert.IsFalse(m.Tables["Internet Sales"].Measures.Any(obj => obj.InPerspective[pnEx]));
            Assert.IsFalse(m.Tables["Product"].Columns.Any(obj => obj.InPerspective[pnEx]));
            Assert.IsFalse(m.Tables["Reseller Sales"].Measures["Reseller Total Sales"].InPerspective[pnEx]);
            Assert.AreEqual(1, m.Tables["Reseller Sales"].Measures.Count(obj => !obj.InPerspective[pnEx]));
            Assert.IsTrue(m.Tables["Reseller Sales"].InPerspective[pnEx]);

            Assert.IsTrue(m.Tables["Geography"].GetChildren().OfType<ITabularPerspectiveObject>().All(obj => obj.InPerspective[pnEx]));
            Assert.IsTrue(m.Tables["Geography"].InPerspective[pnEx]);
            Assert.IsTrue(m.Tables["Date"].Columns.All(obj => obj.InPerspective[pnEx]));

            handler.UndoManager.Rollback();
            handler.SaveDB();

            // Check that perspectives were succesfully deleted:
            Assert.IsFalse(m.Perspectives.Contains(pnIn));
            Assert.IsFalse(m.Perspectives.Contains(pnEx));
        }

        [TestMethod()]
        public void CultureTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            var model = handler.Model;

            var measure = model.Tables["Reseller Sales"].Measures["Reseller Total Sales"];
            measure.DisplayFolder = "Test Folder";

            model.AddTranslation("da-DK");
            Assert.AreEqual("", measure.TranslatedNames["da-DK"]);
            Assert.AreEqual("", measure.TranslatedDisplayFolders["da-DK"]);

            measure.TranslatedNames["da-DK"] = "Reseller Total Sales";
            Assert.AreEqual("Reseller Total Sales", measure.TranslatedNames["da-DK"]);
            measure.Name = "Changed Name";
            handler.SaveDB();

            Assert.AreEqual("Changed Name", measure.TranslatedNames["da-DK"]);
            handler.UndoManager.Undo();
            measure.TranslatedNames["da-DK"] = "XXX";
            measure.Name = "Changed Name";
            handler.SaveDB();
            Assert.AreEqual("XXX", measure.TranslatedNames["da-DK"]);

            measure.TranslatedNames.Reset();
            measure.TranslatedDisplayFolders.Reset();
            handler.SaveDB();
            Assert.AreEqual("", measure.TranslatedNames["da-DK"]);
            Assert.AreEqual(measure.DisplayFolder, measure.TranslatedDisplayFolders["da-DK"]);

            handler.UndoManager.Rollback();
            handler.SaveDB();

            Assert.IsFalse(model.Cultures.Contains("da-DK"));
        }
    }
}