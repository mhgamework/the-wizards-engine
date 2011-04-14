using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.ServerClient
{
    public class TWWereld
    {
        private XNAGame game;

        public XNAGame Game
        {
            get { return game; }
            set { game = value; }
        }

        private IDList<WereldEntity> entities;

        public IDList<WereldEntity> Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        private List<WereldModel> models;

        public List<WereldModel> Models
        {
            get { return models; }
            set { models = value; }
        }
	
	


        public TWWereld( XNAGame nGame )
        {
            game = nGame;
            entities = new IDList<WereldEntity>();
            models = new List<WereldModel>();
        }




        
	
    }
}
