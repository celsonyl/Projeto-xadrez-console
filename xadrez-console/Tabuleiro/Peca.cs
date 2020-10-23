using System;
using System.Collections.Generic;
using System.Text;

namespace tabuleiro
{
    abstract class Peca{
        public Posicao posicao { get; set; }
        public Cor cor { get; protected set; }
        public int quantMovimentos { get; protected set; }
        public Tabuleiro tab { get; protected set; }

        public Peca(Tabuleiro tab,Cor cor)
        {
            this.posicao = null;
            this.tab = tab;
            this.cor = cor;
            this.quantMovimentos = 0;
        }

        public abstract bool[,] movimentosPossiveis();

        public void incrementarQuantidadeMovimento()
        {
            quantMovimentos++;
        }
    }
}
