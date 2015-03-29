using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AjustarPagamentos
{
    public partial class Form1 : Form
    {
        ConexaoDB conexao;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conexao = new ConexaoDB(Properties.Settings.Default.connACP);
        }

        private void btBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                String sql = "select dt_mesRefContribuicao mes,dt_anoRefContribuicao ano " +
                             " ,cd_tipoContribuicao tipo , vl_ContribuicaoVencimento valor from tbContribuicoes ";

                sql += " where cd_cadastro = " + txCodigo.Text;

                sql += " and dt_Contribuicao between '" + formataData(dtInicio.Value.ToShortDateString()) + "' ";
                sql += " and '" + formataData(dtFim.Value.ToShortDateString()) + "'";
                sql += " order by dt_anoRefContribuicao, dt_mesRefContribuicao	  ";

                conexao.abreBanco();

                grid.DataSource = conexao.retornarDataSet(sql);

            }
            finally {
                conexao.fechaBanco();
            }

        }

        private String formataData(String data) {
            String[] dataV = data.Split('/');
            data = dataV[2]+"-"+dataV[1]+"-"+dataV[0];

            return data;
        }

        private void btApagar_Click(object sender, EventArgs e)
        {

            DialogResult result1 = MessageBox.Show("Você deseja realmente apagar \no(s) pagamento(s) selecionado(s)?",
        "Atenção",
        MessageBoxButtons.YesNo);


            if (result1 == System.Windows.Forms.DialogResult.Yes)
            {

                foreach (DataGridViewRow linha in grid.SelectedRows)
                {

                    apagarPagamento(txCodigo.Text,
                                    linha.Cells[2].Value.ToString(),
                                    linha.Cells[0].Value.ToString(),
                                    linha.Cells[1].Value.ToString());

                   
                }

                MessageBox.Show("Pagamento(s) apagado(s)!");
            }
        }

        private void apagarPagamento(String codigo, String tipo, String mes, String ano) {
            try
            {
                conexao.abreBanco();

                String sql = "delete from tbContribuicoes where cd_cadastro =" + codigo +
                             " and cd_tipoContribuicao = " + tipo +
                             " and dt_mesRefContribuicao = " + mes +
                             " and dt_anoRefContribuicao = " + ano;
                
                conexao.executaSemRetorno(sql);

                sql = "delete from tbPagtoContribuicao where cd_cadastro =" + codigo +
                             " and cd_tipoContribuicao = " + tipo +
                             " and dt_mesRefContribuicao = " + mes +
                             " and dt_anoRefContribuicao = " + ano;

                conexao.executaSemRetorno(sql);
            }
            finally {
                conexao.fechaBanco();
            }
        }
    }
}
