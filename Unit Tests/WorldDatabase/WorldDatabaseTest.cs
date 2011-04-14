using System;
using MHGameWork.TheWizards.WorldDatabase;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.WorldDatabase
{
    [TestFixture]
    public class WorldDatabaseTest
    {

        // WorldDatabase
        [Test]
        public void TestSaveLoadDataElement()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestSaveLoadDataElement";
            TheWizards.WorldDatabase.WorldDatabase db;

            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);

            DataItemIdentifier dataItem = new DataItemIdentifier(450);
            DataRevisionIdentifier revision = new DataRevisionIdentifier(13);
            DataElementIdentifier element1Identifier = new DataElementIdentifier("WorlDatabaseTestTerrainData");

            db.RegisterDataElementType(typeof(TerrainDataElement), element1Identifier);


            TerrainDataElementFactory factory = new TerrainDataElementFactory(db);
            db.AddDataElementFactory(factory, true);



            TerrainDataElement dataElement = new TerrainDataElement();
            dataElement.BlockSize = 32;
            dataElement.TerrainSize = 256;


            db.SaveDataElement(dataItem, revision, dataElement);

            DataItem item = new DataItem(db, null, dataItem.Id, DataRevisionIdentifier.WorkingCopy);
            item.SetDataElementChanged(element1Identifier, new DataElementFactoryIdentifier(factory.GetUniqueName()), revision);

            TerrainDataElement dataElement2 = db.LoadDataElement<TerrainDataElement>(item);

            if (dataElement2.Equals(dataElement) == false) throw new Exception();


            dataElement.BlockSize = 16;
            dataElement.TerrainSize = 1024;

            dataItem = new DataItemIdentifier(6000);

            db.SaveDataElement(dataItem, revision, dataElement);

            revision = DataRevisionIdentifier.WorkingCopy;
            dataElement.BlockSize = 32;

            db.SaveDataElement(dataItem, revision, dataElement);



        }

        [Test]
        public void TestSaveLoadRevision()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestSaveLoadRevision";
            TheWizards.WorldDatabase.WorldDatabase db;
            DataItemType type;




            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);

            DataRevision rev;
            DataItem item;


            DataRevision rev1 = rev = new DataRevision(new DataRevisionIdentifier(5));


            item = new DataItem(db, db.FindOrCreateDataItemType("Entity"), 20, rev.Identifier);
            rev.AddDataItem(item);

            item = new DataItem(db, db.FindOrCreateDataItemType("Entity"), 30, rev.Identifier);
            rev.AddDataItem(item);

            db.SaveRevision(rev);





            rev = new DataRevision(new DataRevisionIdentifier(14));

            item = new DataItem(db, db.FindOrCreateDataItemType("Terrain"), 45, rev.Identifier);
            rev.AddDataItem(item);

            item = new DataItem(db, db.FindOrCreateDataItemType("Entity"), 30, rev.Identifier);
            rev.AddDataItem(item);

            db.SaveRevision(rev);





            DataRevision rev3 = db.LoadRevision(new DataRevisionIdentifier(5));


            //TODO: create equals

            if (!rev1.Equals(rev3)) throw new Exception("Loaded revision does not match saved revision!");


        }


        // Working copy

        [Test]
        public void TestModifyWorkingCopy()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestModifyWorkingCopy";
            TheWizards.WorldDatabase.WorldDatabase db;

            DataItem item1, item2, item3;

            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);

            db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Entity"));
            db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Script"));
            item1 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Terrain"));
            db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Terrain"));
            item2 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Entity"));
            item3 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Rommel"));



            db.WorkingCopy.RemoveDataItem(item3);


            TerrainDataElement dataElement;

            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 16;
            db.WorkingCopy.PutDataElement(item1, dataElement);


            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 32;
            db.WorkingCopy.PutDataElement(item2, dataElement);



        }

        [Test]
        public void TestSaveWorkingCopy()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestSaveWorkingCopy";
            TheWizards.WorldDatabase.WorldDatabase db;
            DataItemType type;

            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);

            db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Entity"));
            db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Script"));

            db.WorkingCopy.SaveToDisk();



            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);

            db.RegisterDataElementType(typeof(TerrainDataElement), "WorldDatabaseTest.TerrainDataElement");
            db.RegisterDataElementType(typeof(TerrainData2Element), "WorldDatabaseTest.TerrainData2Element");

            db.AddDataElementFactory(new TerrainDataElementFactory(db), true);
            db.AddDataElementFactory(new TerrainData2ElementFactory(db), true);

            db.WorkingCopy.RemoveDataItem(db.WorkingCopy.Revision.DataItems[1]);
            DataItem item2 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Terrain"));
            DataItem item3 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Entity"));


            TerrainDataElement dataElement;

            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 16;
            db.WorkingCopy.PutDataElement(item2, dataElement);


            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 32;
            db.WorkingCopy.PutDataElement(item3, dataElement);

            TerrainData2Element data2Element = new TerrainData2Element();
            data2Element.SomeData = 42;
            db.WorkingCopy.PutDataElement(item3, data2Element);

            db.WorkingCopy.SaveToDisk();

        }

        [Test]
        public void TestSaveLoadWorkingCopy()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestSaveLoadWorkingCopy";
            TheWizards.WorldDatabase.WorldDatabase db;
            DataItemType type;

            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);

            db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Entity"));
            db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Script"));

            db.WorkingCopy.SaveToDisk();



            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);

            db.RegisterDataElementType(typeof(TerrainDataElement), "WorldDatabaseTest.TerrainDataElement");
            db.RegisterDataElementType(typeof(TerrainData2Element), "WorldDatabaseTest.TerrainData2Element");

            db.AddDataElementFactory(new TerrainDataElementFactory(db), true);
            db.AddDataElementFactory(new TerrainData2ElementFactory(db), true);

            db.WorkingCopy.RemoveDataItem(db.WorkingCopy.Revision.DataItems[1]);
            DataItem item2 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Terrain"));
            DataItem item3 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Entity"));


            TerrainDataElement dataElement;

            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 16;
            db.WorkingCopy.PutDataElement(item2, dataElement);


            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 32;
            db.WorkingCopy.PutDataElement(item3, dataElement);

            TerrainData2Element data2Element = new TerrainData2Element();
            data2Element.SomeData = 42;
            db.WorkingCopy.PutDataElement(item3, data2Element);

            db.WorkingCopy.SaveToDisk();


            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);


            DataItem item1 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Terrain"));
            db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Terrain"));

            db.WorkingCopy.RemoveDataItem(item1);

            db.WorkingCopy.SaveToDisk();


        }

        [Test]
        public void TestCommitWorkingCopy()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestCommitWorkingCopy";
            TheWizards.WorldDatabase.WorldDatabase db;


            db = new TheWizards.WorldDatabase.WorldDatabase(dataDir);

            db.RegisterDataElementType(typeof(TerrainDataElement), "WorldDatabaseTest.TerrainDataElement");
            db.AddDataElementFactory(new TerrainDataElementFactory(db), true);

            DataItem item2 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Terrain"));
            DataItem item3 = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("Entity"));


            TerrainDataElement dataElement;

            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 16;
            db.WorkingCopy.PutDataElement(item2, dataElement);


            //db.WorkingCopy.SaveToDisk();

            db.WorkingCopy.Commit();

        }


        // XML Serializer
        /*public void TestXMLFactory()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestSaveLoadWorkingCopy";
            WorldDatabase db;
            DataItemType type;


            db = new WorldDatabase( dataDir );

            db.RegisterDataElementType( typeof( TerrainDataElement ), "WorldDatabaseTest.TerrainDataElement" );
            db.RegisterDataElementType( typeof( TerrainData2Element ), "WorldDatabaseTest.TerrainData2Element" );

            db.AddDataElementFactory( new TerrainDataElementFactory( db ), true );
            db.AddDataElementFactory( new TerrainData2ElementFactory( db ), true );

            db.WorkingCopy.RemoveDataItem( db.WorkingCopy.Revision.DataItems[ 1 ] );
            DataItem item2 = db.WorkingCopy.CreateNewDataItem( db.FindOrCreateDataItemType( "Terrain" ) );
            DataItem item3 = db.WorkingCopy.CreateNewDataItem( db.FindOrCreateDataItemType( "Entity" ) );


            TerrainDataElement dataElement;

            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 16;
            db.WorkingCopy.PutDataElement( item2, dataElement );


            dataElement = new TerrainDataElement();
            dataElement.BlockSize = 32;
            db.WorkingCopy.PutDataElement( item3, dataElement );

            TerrainData2Element data2Element = new TerrainData2Element();
            data2Element.SomeData = 42;
            db.WorkingCopy.PutDataElement( item3, data2Element );

            db.WorkingCopy.SaveToDisk();


        }*/
     

    }
}