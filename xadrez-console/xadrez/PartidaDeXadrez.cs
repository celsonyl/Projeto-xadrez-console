using System;
using System.Collections.Generic;
using System.Text;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }

        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;

        public bool xeque { get; private set; }


        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            xeque = false;
            colocarPecas();
            terminada = false;
        }

        public Peca executaMovimento(Posicao origem,Posicao destino)
        {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQuantidadeMovimento();
            Peca pecaCapturada = tab.retirarPeca(destino);

            tab.colocarPeca(p, destino);
            if(pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }
            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem,Posicao destino,Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQuantidadeMovimento();
            if(pecaCapturada!= null)
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem);
        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = executaMovimento(origem, destino);

            if (estaEmXeque(jogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você está em xeque ou ficará em xeque!");
            }

            if (estaEmXeque(adversaria(jogadorAtual)))
            {
                xeque = true;
            }
            else
            {
                xeque = false;
            }

            if (estaEmXeque(adversaria(jogadorAtual)))
            {
                terminada = true;
            }
            else
            {

                turno++;
                mudaJogador();
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach(Peca i in capturadas)
            {
                if(i.cor == cor)
                {
                    aux.Add(i);
                }
            }
            return aux;
        }
       
        public HashSet<Peca> pecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca i in pecas)
            {
                if (i.cor == cor)
                {
                    aux.Add(i);
                }
            }
            aux.ExceptWith(pecasCapturadas(cor));
            return aux;
        }    

       

        public void validarPosicaoOrigem(Posicao pos)
        {
            if(tab.peca(pos) == null)
            {
                throw new TabuleiroException("Não existe uma peça na posição de origem escolhida");
            }
            if(jogadorAtual != tab.peca(pos).cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua ");
            }
            if (!tab.peca(pos).existeMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos possiveis na peça de origem escolhida");
            }
        }

        public void validarPosDestino(Posicao origem,Posicao destino)
        {
            if (!tab.peca(origem).podeMoverPara(destino))
            {
                throw new TabuleiroException("Posição destino inválida!");
            }
        }

        private void mudaJogador()
        {
            if(jogadorAtual == Cor.Branca)
            {
                jogadorAtual = Cor.Preta;
            }
            else
            {
                jogadorAtual = Cor.Branca;
            }
        }

        public void colocarNovaPeca(char coluna,int linha,Peca peca)
        {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }


        private Cor adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }

        private Peca rei(Cor cor)
        {
            foreach(Peca i in pecasEmJogo(cor))
            {
                if(i is Rei)
                {
                    return i;
                }
            }
            return null;
        }


        public bool estaEmXeque(Cor cor)
        {
            Peca r = rei(cor);
            if(r == null)
            {
                throw new TabuleiroException("Esse erro não era pra acontecer. NÃO TEM REI DA COR+ "+cor+" NO TABULEIRO");
            }
            foreach(Peca i in pecasEmJogo(adversaria(cor)))
            {
                bool[,] mat = i.movimentosPossiveis();
                if (mat[r.posicao.linha, r.posicao.coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool testeXequeMate(Cor cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }
            foreach(Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis();
                for(int i = 0; i < tab.linhas; i++)
                {
                    for(int j = 0; j < tab.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executaMovimento(x.posicao, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(x.posicao, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }

            }
            return true;
                
        }

        private void colocarPecas()
        {
            colocarNovaPeca('c', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('c', 2, new Torre(tab, Cor.Branca));

            colocarNovaPeca('d', 2, new Torre(tab, Cor.Branca));
            colocarNovaPeca('e', 2, new Torre(tab, Cor.Branca));
            colocarNovaPeca('e', 1, new Torre(tab, Cor.Branca));

            colocarNovaPeca('d', 1, new Rei(tab, Cor.Branca));

            colocarNovaPeca('c', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Torre(tab, Cor.Preta));

            colocarNovaPeca('d', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('e', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('e', 8, new Torre(tab, Cor.Preta));

            colocarNovaPeca('d',8, new Rei(tab, Cor.Preta));


        }

    }
}
