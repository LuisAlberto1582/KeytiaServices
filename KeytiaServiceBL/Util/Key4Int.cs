﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.CargaCDR
{

    class Key4Int
    {
        protected int piValor1;
        protected int piValor2;
        protected int piValor3;
        protected int piValor4;

        public Key4Int()
        {
            this.piValor1 = 0;
            this.piValor2 = 0;
            this.piValor3 = 0;
            this.piValor4 = 0;
        }

        public Key4Int(int piValor1, int piValor2, int piValor3, int piValor4)
        {
            this.piValor1 = piValor1;
            this.piValor2 = piValor2;
            this.piValor3 = piValor3;
            this.piValor4 = piValor4;
        }


        public int Valor1
        {
            get { return piValor1; }
            set { piValor1 = value; }
        }

        public int Valor2
        {
            get { return piValor2; }
            set { piValor2 = value; }
        }

        public int Valor3
        {
            get { return piValor3; }
            set { piValor3 = value; }
        }

        public int Valor4
        {
            get { return piValor4; }
            set { piValor4 = value; }
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Key4Int k = (Key4Int)obj;
            return (piValor1 == k.piValor1) && (piValor2 == k.piValor2) && (piValor3 == k.piValor3) && (piValor4 == k.piValor4);
        }

        public override int GetHashCode()
        {
            return piValor1 ^ piValor2 ^ piValor3 ^ piValor4;
        }

    }

}