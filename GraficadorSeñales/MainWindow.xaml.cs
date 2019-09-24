using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraficadorSeñales
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void BtnGraficar_Click(object sender, RoutedEventArgs e)
        {
            
            double tiempoInicial =
                double.Parse(txtTiempoInicial.Text);
            double tiempoFinal =
                double.Parse(txtTiempoFinal.Text);
            double frecuenciaMuestreo =
                double.Parse(txtFrecuenciaMuestreo.Text);


            Señal señal;
            Señal señalResultante;

            switch(cbTipoSeñal.SelectedIndex)
            {
                case 0: // Parabolica
                    señal = new SeñalParabolica();
                
                
                    break;
                case 1: //Senoidal
                    double amplitud =
                        double.Parse(
                    ((ConfiguracionSeñalSenoidal)
                        (panelConfiguracion.Children[0])).txtAmplitud.Text);
                    double fase =
                        double.Parse(
                            ((ConfiguracionSeñalSenoidal)
                            (panelConfiguracion.Children[0])).txtFase.Text
                            );
                    double frecuencia =
                        double.Parse(
                            ((ConfiguracionSeñalSenoidal)
                            (panelConfiguracion.Children[0])).txtFrecuencia.Text
                            );
                    señal = 
                        new SeñalSenoidal(amplitud, fase, frecuencia);
                    
                    break;
                case 2:
                    string rutaArchivo =
                        ((ConfiguracionAudio)
                        (panelConfiguracion.Children[0])).txtRutaArchivo.Text;
                    señal = new SeñalAudio(rutaArchivo);
                    txtTiempoInicial.Text =
                        señal.TiempoInicial.ToString();
                    txtTiempoFinal.Text =
                        señal.TiempoFinal.ToString();
                    txtFrecuenciaMuestreo.Text =
                        señal.FrecuenciaMuestreo.ToString();
                    break;
                default:
                    señal = null;
                    break;
            }

            if (cbTipoSeñal.SelectedIndex != 2 
                && señal != null)
            {
                señal.TiempoInicial =
                        tiempoInicial;
                señal.TiempoFinal =
                    tiempoFinal;
                señal.FrecuenciaMuestreo =
                    frecuenciaMuestreo;

                señal.construirSeñal();
            }

            switch(cbOperacion.SelectedIndex)
            {
                case 0: //Escala de amplitud
                    double factorEscala =
                        double.Parse(
                        ((OperacionEscalaAmplitud)
                        (panelConfiguracionOperacion.
                            Children[0])).txtFactorEscala
                            .Text);
                    señalResultante =
                        Señal.escalarAmplitud(señal,
                        factorEscala);
                    break;
                case 1: //Desplazamiento de amplitud
                    double cantidadDesplazamiento =
                        double.Parse(
                        ((OperacionDesplazamientoAmplitud)
                        (panelConfiguracionOperacion.
                            Children[0])).txtCantidadDesplazamiento
                            .Text);
                    señalResultante =
                        Señal.desplazarAmplitud(señal,
                        cantidadDesplazamiento);
                    break;
                default:
                    señalResultante = null;
                    break;
            }
            double amplitudMaxima =
                (señal.AmplitudMaxima >= señalResultante.AmplitudMaxima) ?
                señal.AmplitudMaxima : señalResultante.AmplitudMaxima;
            


            plnGrafica.Points.Clear();
            plnGraficaResultante.Points.Clear();


            foreach (Muestra muestra in señal.Muestras)
            {
                plnGrafica.Points.Add(
                    adaptarCoordenadas(muestra.X,
                    muestra.Y, tiempoInicial, amplitudMaxima)
                    );
            }
            foreach(Muestra muestra in señalResultante.Muestras)
            {
                plnGraficaResultante.Points.Add(
                    adaptarCoordenadas(muestra.X,
                    muestra.Y,tiempoInicial, amplitudMaxima)
                    );
            }


            lblLimiteSuperior.Text =
                amplitudMaxima.ToString("F");
            lblLimiteInferior.Text =
                "-" + amplitudMaxima.ToString("F");

            lblLimiteInferiorResultante.Text = "-" +
                amplitudMaxima.ToString("F");
            lblLimiteSuperiorResultante.Text =
                amplitudMaxima.ToString("F");

            //Original
            plnEjeX.Points.Clear();
            plnEjeX.Points.Add(
                adaptarCoordenadas(tiempoInicial, 0.0,
                tiempoInicial, amplitudMaxima)
                );
            plnEjeX.Points.Add(
                adaptarCoordenadas(tiempoFinal, 0.0, tiempoInicial,
                amplitudMaxima)
                );
            //Resultado
            plnEjeXResultante.Points.Clear();
            plnEjeXResultante.Points.Add(
                adaptarCoordenadas(tiempoInicial, 0.0,
                tiempoInicial, amplitudMaxima));
            plnEjeXResultante.Points.Add(
                adaptarCoordenadas(tiempoFinal, 0.0,
                tiempoInicial, amplitudMaxima));


            //Original
            plnEjeY.Points.Clear();
            plnEjeY.Points.Add(
                adaptarCoordenadas(0.0, amplitudMaxima,
                tiempoInicial,amplitudMaxima));
            plnEjeY.Points.Add(
                adaptarCoordenadas(0.0, -amplitudMaxima,
                tiempoInicial, amplitudMaxima)) ;
            //Resultado
            plnEjeYResultante.Points.Clear();
            plnEjeYResultante.Points.Add(
                adaptarCoordenadas(0.0, amplitudMaxima,
                tiempoInicial, amplitudMaxima)
                );
            plnEjeYResultante.Points.Add(
                adaptarCoordenadas(0.0, -amplitudMaxima,
                tiempoInicial, amplitudMaxima)
                );


        }

        public Point adaptarCoordenadas(
            double x, double y, double tiempoInicial,
            double amplitudMaxima)
        {
            return new Point((x - tiempoInicial) * scrGrafica.Width,
                (-1 * (
                y * ((( scrGrafica.Height  / 2.0  ) -25 ) / amplitudMaxima) )) +
                (scrGrafica.Height / 2.0) );
        }

        private void CbTipoSeñal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panelConfiguracion.Children.Clear();
            switch(cbTipoSeñal.SelectedIndex)
            {
                case 0: //Parabolica
                    break;
                case 1: //Senoidal
                    panelConfiguracion.Children.Add(
                        new ConfiguracionSeñalSenoidal());
                    break;
                case 2:
                    panelConfiguracion.Children.Add(
                        new ConfiguracionAudio()
                        ) ;
                    break;
                default:
                    break;
            }
        }

        private void CbOperacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panelConfiguracionOperacion.Children.Clear();
            switch(cbOperacion.SelectedIndex)
            {
                case 0: //Escala de amplitud
                    panelConfiguracionOperacion.
                        Children.Add(
                            new OperacionEscalaAmplitud()
                        );
                    break;
                case 1: //Desplazamiento de amplitud
                    panelConfiguracionOperacion.
                        Children.Add(
                            new OperacionDesplazamientoAmplitud()
                        );
                    break;
                default:
                    break;
            }
        }
    }
}
