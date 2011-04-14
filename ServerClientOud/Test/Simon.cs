using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Test
{
    public class Simon
    {

        public bool GaatDood( Dier dier )
        {

            dier.leeftijd = dier.leeftijd + 3;

            if ( dier.leeftijd > 9 )
            {
                dier.levend = false;
                return false;
            }
            else
            {
            }




        }

        public void WordtZiek( Dier dier )
        {
        }

        public void Start()
        {
            bool ishijdood;
            ishijdood = GaatDood( 8, true );

            Dier kat = new Dier();

            kat.Slaap();
        }

    }

    public class Dier
    {
        private int leeftijd;
        public bool levend;
        private float percentZiek;
        public string naam;
        public string dierSoort;


        public void Eet()
        {
        }

        public void Slaap(int seconden)
        {
            percentZiek = percentZiek - 0.5;
        }

        public void WordOuder()
        {

        }

    }
}
