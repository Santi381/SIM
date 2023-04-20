using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tp5_Colas.Clases;

namespace Tp5_Colas
{
    public partial class Principal : Form
    {
        public int selectorMesa = 0;
        public Cliente cliente;
        public List<Mesa> mesas = new List<Mesa>();
        private int indice = 0;
        public bool mesaLibre = true;
        public int idCliente = 1;
        public int idFinAtencion = 1;
        
        int iteracion = 0;
        int cantidadDeClientesqueNoIngresaronAlSistema = 0;

        public Principal()
        {
            InitializeComponent();

        }
        public void Form1_Load(object sender, EventArgs e)
        {
            CrearMesas();

        }

        public void CrearMesas()
        {
            var Mesa1 = new Mesa();
            var Mesa2 = new Mesa();
            var Mesa3 = new Mesa();
            var Mesa4 = new Mesa();
            var Mesa5 = new Mesa();
            var Mesa6 = new Mesa();


            mesas.Add(Mesa1);
            mesas.Add(Mesa2);
            mesas.Add(Mesa3);
            mesas.Add(Mesa4);
            mesas.Add(Mesa5);
            mesas.Add(Mesa6);


            int ID = 1;

            foreach (var mesa in mesas)
            {
                mesa.mesaId = ID;
                mesa.contadorClientes = 0;

                ID += 1;
            }

        }

        public int buscarMesaLibre(List<Mesa> mesas)
        {
            int id = -1;
            for (int i = 0; i < 6; i++)
            {
                if (mesas[i].estado == false)
                {
                    id = mesas[i].mesaId;
                    break;
                }
            }
            return id;
        }

        List<FinDeAtencion> listaFinAtencion = new List<FinDeAtencion>();
        Queue<Cliente> ColaEnBarra = new Queue<Cliente>();
        VectorEstado L1 = new VectorEstado();
        List<VectorEstado> listaDeVectores = new List<VectorEstado>();
        Cliente clienteDefault = new Cliente();
        List<Cliente> clientess = new List<Cliente>();
        private void BtnIniciarSimulacion_Click(object sender, EventArgs e)
        {
            if ((textBox2.Text == "" || Convert.ToInt32(textBox2.Text) <= 0 || Convert.ToInt32(textBox2.Text) > Convert.ToInt32(TxtTiempoSimulacion.Text)) )
            {
                MessageBox.Show("Ingrese un dia correcto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int dia = 1;
                int cantClientesQDesistieron = 0;
                int cantClientesAtendidos = 0;
                int cantGrupos = 0;
                int CantGruposQueNoIngresaronAlSistema = 0;
                double cantPromedioDeTiempoDeAtencion = 0;
                double TiempoDeEsperaEnCola = 0;
                int contadorClientesEnCola = 0;
                double impaciencia = 0;
                double cantidadClieEnColaPorHora = 0;
                double tiempoBloqueLlegadas = 0;
                int cantBloqueosLlegadas = 0;
                int cantGruposNoEntraronPorBloqueo = 0;
                int cantClieEnCola = 0;
                bool hayBloqueo = false;
                bool esBloqueoLlegada = false;
                bool esBloqueoBarra = false;
                DgvPrincipal.Rows.Clear();
                DgvClientes.Rows.Clear();
                dataGridView1.Rows.Clear();
                DgvClientes.Columns.Clear();
              
               
                ColaEnBarra.Clear();
                listaFinAtencion.Clear();
                listaDeVectores.Clear();
                clientess.Clear();
                double tiempoSimulacion = 120;
                double inicioSimulacion = 0;
                iteracion = 0;
                listaFinAtencion.Capacity = 6;
                // Creacion de la lista de vectores para trabajar en el vector estado

                DataGridViewTextBoxColumn colNombre = new DataGridViewTextBoxColumn();
                colNombre.Name = "Iteración";

                colNombre.Width = 55;
                colNombre.FillWeight = 100;
                DgvClientes.Columns.Add(colNombre);
                //VectorEstado L2 = new VectorEstado();

                L1.reloj = inicioSimulacion;
                L1.mesas = mesas;
                L1.nombreEvento = "Inicialización";
                //L2.mesas = mesas;

                L1.clientes = clientess;
                //L2.clientes = new List<Cliente>();



                clienteDefault.CantidadCliente = 0;

                listaDeVectores.Add(L1);
                //listaDeVectores.Add(L2);

                int indice = 0;
                bool primera = true;
                bool noHayLlegadas = false;
                

                int quedanClientes = 0;


                for (int h = 0; h < Convert.ToInt32(TxtTiempoSimulacion.Text); h++)
                {
                    while (inicioSimulacion <= tiempoSimulacion || quedanClientes != 0 && primera == false)
                    {
                        if (tiempoSimulacion <= inicioSimulacion)
                        {
                            noHayLlegadas = true;
                        }
                        //primera iteracion solo se van a cargar la llegada y los clientes porque los fin de atencion solo es cuando llega un cliente o entra un cliente desde la barra

                        if (primera == true)
                        {

                            LlegadaCliente proximaLlegada = this.generarLlegadaCliente(inicioSimulacion);
                            ProximoBloqueo proximoBloqueo = this.generarNuevoBloqueo(inicioSimulacion);
                            listaDeVectores[0].nombreEvento = "Inicialización";
                            listaDeVectores[0].reloj = inicioSimulacion;
                            listaDeVectores[0].llegadaCliente = proximaLlegada;
                            listaDeVectores[0].proximoBloqueo = proximoBloqueo;
                            if (dia == Convert.ToInt32(textBox2.Text))
                            {
                                this.cargarGrillas(listaDeVectores, ColaEnBarra);
                            }
                            
                            primera = false;
                            
                        }

                        //no es primera iteracion Vamos a comparar todos los eventos para saber cual es el que tiene menor tiempo
                        else
                        {
                            if (TxtImpaciencia.Text != "")
                            {
                                impaciencia = Convert.ToInt32(TxtImpaciencia.Text);
                            }
                            else
                            {
                                impaciencia = 15;
                            }
                            if (ColaEnBarra.Count > 0)
                            {
                                var a = ColaEnBarra.ToList();
                                for (int i = 0; i < a.Count; i++)
                                {
                                    if (inicioSimulacion - a[i].HorarioLlegada >= impaciencia)
                                    {
                                        ColaEnBarra.Dequeue();
                                        cantClientesQDesistieron += a[i].CantidadCliente;
                                        a[i] = null;

                                    }
                                }
                            }



                            //obtengo el menor fin de atencion
                            FinDeAtencion menor = new FinDeAtencion();
                            // si se han creado fines de atencion buscamos con el menor tiempo y lo guardamos en el menor
                            if (listaFinAtencion.Count != 0)
                            {
                                double menorr = listaFinAtencion.Min(x => x.tiempoFinAtencion);
                                menor = listaFinAtencion.Find(x => x.tiempoFinAtencion == menorr);
                            }
                            else
                            {
                                menor.tiempoFinAtencion = 10000;
                            }


                            // esta lista creada es donde se van a agregar todos eventos en curso para determiar el menor
                            var listaTiempos = new List<double>();

                            // Gruarda el bolqueo anterior
                            var bloqueoAnterior = listaDeVectores[0].proximoBloqueo;

                            finBloqueoLlegadaCliente bloqueoLlegadaAnterior = new finBloqueoLlegadaCliente();
                            finBloqueoBarra bloqueoBarraAnterior = new finBloqueoBarra();

                            // pregunta que tipo de bloqueo y con el tipo de bloqueo guarda el  tiempo
                            if (hayBloqueo == true)
                            {
                                if (esBloqueoLlegada == true)
                                {
                                    bloqueoLlegadaAnterior = listaDeVectores[0].proximoBloqueo.finBloqueoLlegadaCliente;
                                    listaTiempos.Add(bloqueoLlegadaAnterior.tiempoBloqueoLlegadaCliente);
                                }
                                else if (esBloqueoBarra == true)
                                {
                                    bloqueoBarraAnterior = listaDeVectores[0].proximoBloqueo.finBloqueoBarra;
                                    listaTiempos.Add(bloqueoBarraAnterior.tiempoFinBloqueoMesa);

                                }
                            }

                            //asigno la proxima llegada en la variable
                            var llegadaAnterior = listaDeVectores[0].llegadaCliente;

                            //se obtiene el menor tiempo y su evento asociado
                            
                            if (menor.tiempoFinAtencion != 10000)
                            {
                                listaTiempos.Add(menor.tiempoFinAtencion);
                            }

                            if (bloqueoAnterior.TiempoProximBloqueo <= 120 && hayBloqueo == false)
                            {
                                listaTiempos.Add(bloqueoAnterior.TiempoProximBloqueo);
                            }
                            
                            // agrega la prxima llegada a la lsita de tiempos
                            if (llegadaAnterior.proximaLlegada <=120)
                            {
                                listaTiempos.Add(llegadaAnterior.proximaLlegada);
                            }
                            if (listaTiempos.Count ==0)
                            {
                                quedanClientes = 0;
                                continue;
                            }

                            // de la lista de los tiempor busca el menor tiempo
                            var menorTiempo = listaTiempos.Min();



                            if (menorTiempo == llegadaAnterior.proximaLlegada && menorTiempo <= 120)
                            {
                                //el evento es una llegada de cliente

                                if (esBloqueoLlegada == true)
                                {
                                    // como esta bloqueada las llegadas solo se muestra la llagada el tiempo
                                    listaDeVectores[0].reloj = llegadaAnterior.proximaLlegada;
                                    inicioSimulacion = llegadaAnterior.proximaLlegada;
                                    llegadaAnterior = this.generarLlegadaCliente(llegadaAnterior.proximaLlegada);
                                    cantGruposNoEntraronPorBloqueo += 1;
                                    listaDeVectores[0].nroIteracion = iteracion;
                                    listaDeVectores[0].nombreEvento = "Llegada cliente";
                                    listaDeVectores[0].llegadaCliente = llegadaAnterior;
                                    listaDeVectores[0].finDeAtenciones = listaFinAtencion;

                                    if (dia == Convert.ToInt32(textBox2.Text))
                                    {
                                        
                                        this.cargarGrillasLlegadaCliente1(listaDeVectores, ColaEnBarra);
                                    }

                                    iteracion++;
                                    continue;

                                }
                                if (esBloqueoBarra == true)
                                {
                                    var relojjj = llegadaAnterior.proximaLlegada;
                                    listaDeVectores[0].reloj = relojjj;
                                    inicioSimulacion = relojjj;
                                    var nuevoClienteee = this.generarCliente(llegadaAnterior.proximaLlegada);
                                    clientess.Add(nuevoClienteee);
                                    llegadaAnterior.cliente = nuevoClienteee;
                                    llegadaAnterior = this.generarLlegadaCliente(relojjj);
                                    
                                    var idMesaa = this.buscarMesaLibre(mesas);
                                    if (idMesaa != -1)
                                    {
                                        //ubica al cliente en la mesa libre
                                        var mesaLibre = mesas.Find(x => x.mesaId == idMesaa);
                                        mesaLibre.mesaId = idMesaa;
                                        mesaLibre.estado = true;
                                        mesaLibre.cliente = nuevoClienteee;
                                        var finAtencionCli = this.generarFinAtencion(nuevoClienteee, mesaLibre, llegadaAnterior.proximaLlegada);
                                        if (listaFinAtencion.Count == 6)
                                        {
                                            var index = listaFinAtencion.FindIndex(x => x.tiempoFinAtencion == 10000);
                                            listaFinAtencion.RemoveAt(index);
                                            listaFinAtencion.Insert(index, finAtencionCli);
                                        }
                                        else
                                        {
                                            listaFinAtencion.Add(finAtencionCli);
                                        }

                                        cantClientesAtendidos += nuevoClienteee.CantidadCliente;
                                        cantGrupos += 1;
                                        cantPromedioDeTiempoDeAtencion += finAtencionCli.tiempoEvento();
                                    }
                                    else
                                    {
                                        cantidadDeClientesqueNoIngresaronAlSistema += nuevoClienteee.CantidadCliente;
                                        listaDeVectores[0].llegadaCliente = llegadaAnterior;
                                        iteracion++;
                                        continue;
                                    }
                                    iteracion++;
                                    quedanClientes += 1;


                                    listaDeVectores[0].nroIteracion = iteracion;
                                    listaDeVectores[0].nombreEvento = "Llegada cliente C" + nuevoClienteee.clienteId.ToString();
                                    listaDeVectores[0].llegadaCliente = llegadaAnterior;
                                    listaDeVectores[0].proximoBloqueo = bloqueoAnterior;
                                    listaDeVectores[0].llegadaCliente.cliente = nuevoClienteee;
                                    listaDeVectores[0].finDeAtenciones = listaFinAtencion;
                                    listaDeVectores[0].clientes = clientess;

                                    int calculTiempoo = this.calcularTiempo(listaDeVectores);


                                    if (dia == Convert.ToInt32(textBox2.Text))
                                    {
                                        this.AgregarColumna("Estado " + nuevoClienteee.clienteId.ToString(), "Hora llegada " + nuevoClienteee.clienteId.ToString(), "Cantidad");
                                        this.cargarGrillasLlegadaCliente(listaDeVectores, ColaEnBarra, calculTiempoo);
                                    }
                                }

                                var reloj = llegadaAnterior.proximaLlegada;
                                listaDeVectores[0].reloj = reloj;
                                inicioSimulacion = reloj;
                                var nuevoCliente = this.generarCliente(llegadaAnterior.proximaLlegada);
                                clientess.Add(nuevoCliente);
                                llegadaAnterior.cliente = nuevoCliente;
                                llegadaAnterior = this.generarLlegadaCliente(reloj);

                                var idMesa = this.buscarMesaLibre(mesas);
                                if (idMesa != -1)
                                {
                                    //ubica al cliente en la mesa libre
                                    Mesa mesaLibre = mesas.Find(x => x.mesaId == idMesa);
                                    mesaLibre.mesaId = idMesa;
                                    mesaLibre.estado = true;
                                    mesaLibre.cliente = nuevoCliente;
                                    FinDeAtencion finAtencionCli = this.generarFinAtencion(nuevoCliente, mesaLibre, llegadaAnterior.proximaLlegada);
                                    if (listaFinAtencion.Count == 6)
                                    {
                                        var index = listaFinAtencion.FindIndex(x => x.tiempoFinAtencion == 10000);
                                        listaFinAtencion.RemoveAt(index);
                                        listaFinAtencion.Insert(index, finAtencionCli);
                                    }
                                    else
                                    {
                                        listaFinAtencion.Add(finAtencionCli);
                                    }
                                    cantClientesAtendidos += nuevoCliente.CantidadCliente;
                                    cantGrupos += 1;
                                    cantPromedioDeTiempoDeAtencion += finAtencionCli.tiempoEvento();
                                }
                                else
                                {
                                    int cantClientesEnBarra = 0;
                                    foreach (var cliente in ColaEnBarra)
                                    {
                                        cantClientesEnBarra += cliente.CantidadCliente;
                                    }
                                    if ((cantClientesEnBarra + nuevoCliente.CantidadCliente) < 16)
                                    {
                                        ColaEnBarra.Enqueue(nuevoCliente);
                                        nuevoCliente.estado = true;
                                        cantClieEnCola += nuevoCliente.CantidadCliente;
                                    }
                                    else
                                    {
                                        cantidadDeClientesqueNoIngresaronAlSistema += nuevoCliente.CantidadCliente;
                                        nuevoCliente = null;
                                        listaDeVectores[0].llegadaCliente = llegadaAnterior;
                                        iteracion++;
                                        continue;
                                    }
                                   
                                }

                                iteracion++;
                                quedanClientes += 1;


                                listaDeVectores[0].nroIteracion = iteracion;
                                listaDeVectores[0].nombreEvento = "Llegada cliente C" + nuevoCliente.clienteId.ToString();
                                listaDeVectores[0].llegadaCliente = llegadaAnterior;
                                listaDeVectores[0].proximoBloqueo = bloqueoAnterior;
                                listaDeVectores[0].llegadaCliente.cliente = nuevoCliente;
                                listaDeVectores[0].finDeAtenciones = listaFinAtencion;
                                listaDeVectores[0].clientes = clientess;

                                int calculTiempo = this.calcularTiempo(listaDeVectores);


                                if (dia == Convert.ToInt32(textBox2.Text))
                                {
                                    this.AgregarColumna("Estado " + nuevoCliente.clienteId.ToString(), "Hora llegada " + nuevoCliente.clienteId.ToString(), "Cantidad");
                                    this.cargarGrillasLlegadaCliente(listaDeVectores, ColaEnBarra, calculTiempo);
                                }
                                


                                continue;

                            }

                            else if (menorTiempo == bloqueoAnterior.TiempoProximBloqueo)
                            {
                                //el evento es un bloqueo
                                
                                listaDeVectores[0].reloj = bloqueoAnterior.TiempoProximBloqueo;
                                var tiempo = listaDeVectores[0].reloj;
                                
                                hayBloqueo = true;

                                var tipoBloqueo = bloqueoAnterior.determinarTipoBloqueo();
                                if (tipoBloqueo == "Llegada")
                                {
                                    //bloqueo de llegada
                                    finBloqueoLlegadaCliente bloqueoLlegadaCliente = this.generarBloqueoLlegada(tiempo);
                                    listaDeVectores[0].proximoBloqueo.finBloqueoLlegadaCliente = bloqueoLlegadaCliente;
                                    listaDeVectores[0].proximoBloqueo.TipoBloqueo = tipoBloqueo;
                                    
                                    esBloqueoLlegada = true;
                                    tiempoBloqueLlegadas += bloqueoLlegadaCliente.tiempoBloqueoParcial;
                                    cantBloqueosLlegadas += 1;
                                }
                                else if (tipoBloqueo == "Servidor")
                                {
                                    //bloqueo de bara
                                    finBloqueoBarra bloqueoBarra = this.generarBloqueBarra(tiempo);
                                    listaDeVectores[0].proximoBloqueo.finBloqueoBarra = bloqueoBarra;
                                    listaDeVectores[0].proximoBloqueo.TipoBloqueo = tipoBloqueo;
                                    esBloqueoBarra = true;
                                }
                                listaDeVectores[0].finDeAtenciones = listaFinAtencion;
                                listaDeVectores[0].nombreEvento = "Bloqueo";
                                listaDeVectores[0].llegadaCliente = listaDeVectores[0].llegadaCliente;

                                if (dia == Convert.ToInt32(textBox2.Text))
                                {
                                    this.cargarGrillasBloqueo(listaDeVectores, esBloqueoLlegada);
                                }


                                iteracion++;
                            }      
                            
                            else if (menorTiempo == menor.tiempoFinAtencion && menor.tiempoFinAtencion != 10000)
                            {
                                //el evento es un fin de atencion
                                var reloj = menor.tiempoFinAtencion;
                                listaDeVectores[0].reloj = reloj;
                                inicioSimulacion = reloj;

                                var index = listaFinAtencion.FindIndex(x => x.cliente != null && x.cliente.clienteId == menor.cliente.clienteId);
                                menor.cliente = null;
                                menor.mesa.estado = false;
                                listaFinAtencion.RemoveAt(index);
                                

                                if (ColaEnBarra.Count > 0)
                                {
                                    var cliente = ColaEnBarra.Dequeue();
                                    cliente.estado = false;
                                    menor.mesa.cliente = cliente;
                                    menor.mesa.estado = true;
                                    menor.cliente = cliente;
                                    FinDeAtencion fin = this.generarFinAtencion(cliente, menor.mesa, reloj);
                                    listaFinAtencion.Insert(index, fin);
                                    cantGrupos += 1;
                                    cantClientesAtendidos += cliente.CantidadCliente;
                                    cantPromedioDeTiempoDeAtencion += fin.tiempoEvento();
                                }
                                else
                                {
                                    var finn = new FinDeAtencion();
                                    finn.tiempoFinAtencion = 10000;
                                    listaFinAtencion.Insert(index, finn);
                                }


                                iteracion++;
                                quedanClientes -= 1;

                                listaDeVectores[0].nombreEvento = "Fin de atención";
                                listaDeVectores[0].llegadaCliente = llegadaAnterior;
                                //listaDeVectores[0].proximoBloqueo = bloqueoAnterior;
                                listaDeVectores[0].finDeAtenciones = listaFinAtencion;
                                listaDeVectores[0].clientes = clientess;
                                if (dia == Convert.ToInt32(textBox2.Text))
                                {
                                    this.cargarGrillasFinAtencion(listaDeVectores, ColaEnBarra);
                                }
                                

                                continue;
                            }

                            else if (menorTiempo == bloqueoLlegadaAnterior.tiempoBloqueoLlegadaCliente)
                            {
                                //el evento es un fin de bloqueo de llegada de cliente
                                var reloj = bloqueoLlegadaAnterior.tiempoBloqueoLlegadaCliente;
                                listaDeVectores[0].reloj = reloj;
                                //inicioSimulacion = reloj;
                                hayBloqueo = false;
                                esBloqueoLlegada = false;
                                var proximoBloqueo = this.generarNuevoBloqueo(reloj);
                                listaDeVectores[0].proximoBloqueo = proximoBloqueo;
                                listaDeVectores[0].nombreEvento = "Fin bloqueo llegada";
                                listaDeVectores[0].llegadaCliente = llegadaAnterior;
                                listaDeVectores[0].finDeAtenciones = listaFinAtencion;
                                if (dia == Convert.ToInt32(textBox2.Text))
                                {
                                    this.cargarGrillasFinBloqueoLLegada(listaDeVectores, ColaEnBarra, llegadaAnterior.proximaLlegada);
                                }
                                
                            }

                            else if (menorTiempo == bloqueoBarraAnterior.tiempoFinBloqueoMesa)
                            {
                                //el evento es un fin de bloqueo de barra
                                var reloj = bloqueoBarraAnterior.tiempoFinBloqueoMesa;
                                listaDeVectores[0].reloj = reloj;
                                //inicioSimulacion = reloj;
                                hayBloqueo = false;
                                esBloqueoBarra = false;
                                var proximoBloqueo = this.generarNuevoBloqueo(reloj);
                                listaDeVectores[0].proximoBloqueo = proximoBloqueo;
                                listaDeVectores[0].nombreEvento = "Fin bloqueo barra";
                                listaDeVectores[0].llegadaCliente = llegadaAnterior;
                                listaDeVectores[0].finDeAtenciones = listaFinAtencion;

                                if (dia == Convert.ToInt32(textBox2.Text))
                                {
                                    this.cargarGrillasFinBloqueoLLegada(listaDeVectores, ColaEnBarra, llegadaAnterior.proximaLlegada);
                                }
                                
                            }
                            //quedanClientes = 0;

                            
                        }

                    }

                    //DgvPrincipal.Rows.Clear();
                    //DgvClientes.Rows.Clear();
                    //dataGridView1.Rows.Clear();
                    iteracion = 0;
                    ColaEnBarra.Clear();
                    listaFinAtencion.Clear();
                    listaDeVectores.Clear();
                    clientess.Clear();
                    primera = true;
                    inicioSimulacion = 0;
                    L1.reloj = inicioSimulacion;
                    L1.mesas = mesas;
                    listaDeVectores.Add(L1);
                    if (h + 1 == Convert.ToInt32(textBox2.Text))
                    {
                        var filass = new string[27];
                        filass[0] = "Fin día " + (h + 1).ToString();
                        for (int i = 0; i < 27; i++)
                        {
                            // printeamos los estados de las mesas
                            if (i > 20)
                            {
                                if (mesas[i - 21].estado == false)
                                {
                                    filass[i] = "Libre";
                                }
                                else if (mesas[i - 21].estado == true)
                                {
                                    filass[i] = "Ocupado";
                                }
                            }

                        }
                        DgvPrincipal.Rows.Add(filass);

                        
                    }


                    dia += 1;



                }


                ////cuando termina la simulacion calculamos las metricas

                TxtCnatCAtendidos.Text = (cantClientesAtendidos).ToString();
                TxtContadorDeCNoAt.Text = (cantClientesQDesistieron.ToString());

               
                CantGrupoNoEntraron.Text = cantGruposNoEntraronPorBloqueo.ToString();
                TxtCantPromedioFinAtencion.Text = Math.Round((cantPromedioDeTiempoDeAtencion / cantGrupos), 2).ToString();
                CantClieEnBarraPorHora.Text = (cantClieEnCola / Convert.ToInt32(TxtTiempoSimulacion.Text)).ToString();
                if (tiempoBloqueLlegadas / cantBloqueosLlegadas == 0 || (tiempoBloqueLlegadas == 0 && cantBloqueosLlegadas ==0))
                {
                    TxtCantTiempoBloqBarra.Text = 0.ToString();
                }
                else
                {
                    
                    TxtCantTiempoBloqBarra.Text = Math.Round((tiempoBloqueLlegadas / cantBloqueosLlegadas), 2).ToString();
                }
            }





        }
        private void AgregarColumna(string nombre, string horaLlegada, string cant)
        {
            DataGridViewTextBoxColumn colNombre = new DataGridViewTextBoxColumn();
            colNombre.Name = nombre;
            
            colNombre.Width = 55;
            colNombre.FillWeight = 100;
            DgvClientes.Columns.Add(colNombre);
            DataGridViewTextBoxColumn colHoraLlegada = new DataGridViewTextBoxColumn();
            colHoraLlegada.Name = horaLlegada;
            
            colHoraLlegada.Width = 90;
            colHoraLlegada.FillWeight = 100;
            DgvClientes.Columns.Add(colHoraLlegada);
            DataGridViewTextBoxColumn colCantidad = new DataGridViewTextBoxColumn();
            colCantidad.Name = cant;
            
            colCantidad.Width = 50;
            colCantidad.FillWeight = 100;
            DgvClientes.Columns.Add(colCantidad);

        }

        private Cliente generarCliente(double llegadaAnterior)
        {
            var cliente = new Cliente();
            cliente.clienteId = idCliente;
            idCliente++;
            //cliente.rndCantClientes = cliente.crearRandomCantidad(); este atributo se setea en el emtodo de abajo
            cliente.CantidadCliente = cliente.calcularCantidadClientes();
            cliente.HorarioLlegada = llegadaAnterior;
            return cliente;
        }

        private LlegadaCliente generarLlegadaCliente(double valorReloj)
        {
            //selectorMesa recibe por parametro el valor del reloj actual y se genera la proxima llegada de cliente
            LlegadaCliente llegadaCliente = new LlegadaCliente();
            double rndLlegadaa = llegadaCliente.crearRandomLlegada();
            llegadaCliente.rndLlegada = rndLlegadaa;
            double tiempoLlegadaa = llegadaCliente.tiempoEvento();
            llegadaCliente.tiempoLlegada = tiempoLlegadaa;
            llegadaCliente.proximaLlegada = tiempoLlegadaa + valorReloj;
            return llegadaCliente;
        }

        private FinDeAtencion generarFinAtencion(Cliente cliente, Mesa mesa, double valorReloj)
        {

            // crea un fin de atencion  le asigna un id autoincremental  le asigna el cliente y mesa pasado como parametro  y suma el tiempo de la atencion con el valor del reloj pasado como parametro(tiempo actual) 
            FinDeAtencion finAtencion = new FinDeAtencion();
            finAtencion.idFinEvento = idFinAtencion;
            idFinAtencion++;
            finAtencion.cliente = cliente;
            double tiempo = finAtencion.tiempoEvento();
            finAtencion.tiempoFinAtencion = tiempo + valorReloj;
            finAtencion.mesa = mesa;
            return finAtencion;
        }

        private ProximoBloqueo generarNuevoBloqueo(double reloj)
        {
            ProximoBloqueo proximoBloqueo = new ProximoBloqueo();
            double A = 273.873;
            double t0 = 0;
            double h = 0.1;
            double tiempoBloqueo = proximoBloqueo.tiempoBloqueo(h,t0,0,A);
            proximoBloqueo.TBloqueo = tiempoBloqueo;
            proximoBloqueo.TiempoProximBloqueo = reloj + tiempoBloqueo;
            return proximoBloqueo;
        }

        private finBloqueoLlegadaCliente generarBloqueoLlegada(double reloj)
        {
            finBloqueoLlegadaCliente bloqueoLlegada = new finBloqueoLlegadaCliente();
            double tiempoFinBloqueo = bloqueoLlegada.tiempoBloqueo(0.1, 0, 0, reloj);
            bloqueoLlegada.tiempoBloqueoParcial = tiempoFinBloqueo;
            bloqueoLlegada.tiempoBloqueoLlegadaCliente = tiempoFinBloqueo + reloj;
            return bloqueoLlegada;
        }

        private finBloqueoBarra generarBloqueBarra(double reloj)
        {
            finBloqueoBarra bloqueoBarra = new finBloqueoBarra();
            double tiempoFinBloqueo = bloqueoBarra.tiempoBloqueo(0.1, 0, 0, reloj);
            bloqueoBarra.tiempoBloqueoParcial = tiempoFinBloqueo;
            bloqueoBarra.tiempoFinBloqueoMesa = tiempoFinBloqueo + reloj;
            return bloqueoBarra;
        }

        private int calcularTiempo(List<VectorEstado> listaDeVectores)
        {
            int calculTiempo = 0;
            switch (listaDeVectores[0].llegadaCliente.cliente.CantidadCliente)
            {
                case 1:
                    calculTiempo = 20;
                    break;
                case 2:
                    calculTiempo = 30;
                    break;
                case 3:
                    calculTiempo = 40;
                    break;
                case 4:
                    calculTiempo = 40;
                    break;
                default:
                    calculTiempo = 0;
                    break;
            
            }
            return calculTiempo;
        }

        private void cargarGrillasLlegadaCliente2(List<VectorEstado> listaDeVectores, Queue<Cliente> ColaEnBarra, double llegadaClienteAnterior)
        {

            //CARGAMOS LOS DATOS DE LA GRILLA PRINCIPAL
            var filass = new string[27];
            filass[0] = iteracion.ToString();
            filass[1] = listaDeVectores[0].nombreEvento;
            filass[2] = listaDeVectores[0].reloj.ToString();
            filass[3] = listaDeVectores[0].llegadaCliente.rndLlegada.ToString();
            filass[4] = listaDeVectores[0].llegadaCliente.tiempoLlegada.ToString();
            filass[5] = listaDeVectores[0].llegadaCliente.proximaLlegada.ToString();
            filass[7] = listaDeVectores[0].proximoBloqueo.TiempoProximBloqueo.ToString();
            filass[12] = listaDeVectores[0].llegadaCliente.cliente.rndCantClientes.ToString();
            filass[13] = listaDeVectores[0].llegadaCliente.cliente.CantidadCliente.ToString();
            foreach (var finAtencion in listaDeVectores[0].finDeAtenciones)
            {
                double tiempox = finAtencion.tiempoFinAtencion - llegadaClienteAnterior;
                filass[14] = tiempox.ToString();

            }
            for (int i = 0; i < 27; i++)
            {
                for (int g = 0; g < listaDeVectores[0].finDeAtenciones.Count; g++)
                {
                    if (listaDeVectores[0].finDeAtenciones[g].tiempoFinAtencion >= 10000)
                    {
                        filass[15 + g] = "";
                    }
                    else
                    {
                        filass[15 + g] = listaDeVectores[0].finDeAtenciones[g].tiempoFinAtencion.ToString();
                    }

                }

                // printeamos los estados de las mesas
                if (i > 20)
                {
                    if (mesas[i - 21].estado == false)
                    {
                        filass[i] = "Libre";
                    }
                    else if (mesas[i - 21].estado == true)
                    {
                        filass[i] = "Ocupado";
                    }
                }

            }
            DgvPrincipal.Rows.Add(filass);

            // CARGAMOS LOS DATOS DE LA GRILLA DE LA BARRA
            var filas2 = new string[16];
            filas2[0] = iteracion.ToString();
            if (ColaEnBarra.Count != 0)
            {
                int lugaresOcupadosEnBarra = 0;
                foreach (var cliente in ColaEnBarra)
                {
                    lugaresOcupadosEnBarra += cliente.CantidadCliente;
                }
                for (int i = 1; i < lugaresOcupadosEnBarra; i++)
                {
                    filas2[i] = "Ocupado";
                }
                for (int i = lugaresOcupadosEnBarra; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }

            }
            else
            {

                for (int i = 1; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }
            }
            dataGridView1.Rows.Add(filas2);

            // CARGAMOS LOS DATOS DE LOS CLIENTES

            var cantidades = listaDeVectores[0].clientes.Count * 3;
            var filas3 = new string[cantidades + 1];
            filas3[0] = iteracion.ToString();
            var fil1 = 1;
            var fil2 = 2;
            var fil3 = 3;
            for (int i = 0; i < listaDeVectores[0].clientes.Count; i++)
            {
                for (int g = 0; g < listaDeVectores[0].clientes.Count; g++)
                {
                    if ((listaDeVectores[0].clientes[g].estado) == true)
                    {
                        filas3[fil1] = "EnBarra";
                        fil1 += 3;
                    }
                    else
                    {
                        filas3[fil1] = "Atendido";
                        fil1 += 3;
                    }

                    filas3[fil2] = listaDeVectores[0].clientes[g].HorarioLlegada.ToString();
                    filas3[fil3] = listaDeVectores[0].clientes[g].CantidadCliente.ToString();
                    fil3 += 3;
                    fil2 += 3;

                }
                fil1 = 1;
                fil2 = 2;
                fil3 = 3;
            }
            DgvClientes.Rows.Add(filas3);
        }

        private void cargarGrillasFinBloqueo(List<VectorEstado> listaDeVectores, Queue<Cliente> ColaEnBarra, double llegadaClienteAnterior)
        {
            var filass = new string[27];
            filass[0] = iteracion.ToString();
            filass[1] = listaDeVectores[0].nombreEvento;
            filass[2] = listaDeVectores[0].reloj.ToString();
            filass[5] = llegadaClienteAnterior.ToString();
            filass[6] = listaDeVectores[0].proximoBloqueo.TBloqueo.ToString();
            filass[7] = listaDeVectores[0].proximoBloqueo.TiempoProximBloqueo.ToString();

            for (int i = 0; i < 27; i++)
            {
                // printeamos los estados de las mesas
                if (i > 20)
                {
                    if (mesas[i - 21].estado == false)
                    {
                        filass[i] = "Libre";
                    }
                    else if (mesas[i - 21].estado == true)
                    {
                        filass[i] = "Ocupado";
                    }
                }

            }
            DgvPrincipal.Rows.Add(filass);

            //printeeamos el estado de la barra como es la primera iteracion esta todo libre
            var filas2 = new string[16];
            filas2[0] = iteracion.ToString();
            if (ColaEnBarra.Count != 0)
            {
                int lugaresOcupadosEnBarra = 0;
                foreach (var cliente in ColaEnBarra)
                {
                    lugaresOcupadosEnBarra += cliente.CantidadCliente;
                }
                for (int i = 1; i < lugaresOcupadosEnBarra; i++)
                {
                    filas2[i] = "Ocupado";
                }
                for (int i = lugaresOcupadosEnBarra; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }

            }
            else
            {

                for (int i = 1; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }
            }
            dataGridView1.Rows.Add(filas2);

            // mostrar clientes del sistema  Como es la primera iteracion no vamos a tener
            var cantidades = listaDeVectores[0].clientes.Count * 2;
            var filas3 = new string[cantidades + 1];
            filas3[0] = iteracion.ToString();
            var fil1 = 1;
            var fil2 = 2;
            for (int i = 1; i < listaDeVectores[0].clientes.Count; i++)
            {
                for (int g = 1; g < listaDeVectores[0].clientes.Count; g++)
                {
                    if ((listaDeVectores[0].clientes[g].estado) == true)
                    {
                        filas3[fil1] = "EnBarra";
                        fil1 += 2;
                    }
                    else
                    {
                        filas3[fil1] = "Atendido";
                        fil1 += 2;
                    }

                    filas3[fil2] = listaDeVectores[0].clientes[g].HorarioLlegada.ToString();
                    fil2 += 2;

                }
                fil1 = 1;
                fil2 = 2;
            }
            DgvClientes.Rows.Add(filas3);
        }

        private void cargarGrillasFinBloqueoLLegada(List<VectorEstado> listaDeVectores, Queue<Cliente> ColaEnBarra, double llegadaClienteAnterior)
        {
            var filass = new string[27];
            filass[0] = iteracion.ToString();
            filass[1] = listaDeVectores[0].nombreEvento;
            filass[2] = listaDeVectores[0].reloj.ToString();
            filass[5] = llegadaClienteAnterior.ToString();
            filass[6] = listaDeVectores[0].proximoBloqueo.TBloqueo.ToString();
            filass[7] = listaDeVectores[0].proximoBloqueo.TiempoProximBloqueo.ToString();

            for (int i = 0; i < 27; i++)
            {
                // printeamos los estados de las mesas
                if (i > 20)
                {
                    if (mesas[i - 21].estado == false)
                    {
                        filass[i] = "Libre";
                    }
                    else if (mesas[i - 21].estado == true)
                    {
                        filass[i] = "Ocupado";
                    }
                }

            }
            DgvPrincipal.Rows.Add(filass);

            //printeeamos el estado de la barra como es la primera iteracion esta todo libre
            var filas2 = new string[16];
            filas2[0] = iteracion.ToString();
            if (ColaEnBarra.Count != 0)
            {
                int lugaresOcupadosEnBarra = 0;
                foreach (var cliente in ColaEnBarra)
                {
                    lugaresOcupadosEnBarra += cliente.CantidadCliente;
                }
                for (int i = 1; i < ColaEnBarra.Count; i++)
                {
                    filas2[i] = "Ocupado";
                }
                for (int i = ColaEnBarra.Count; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }

            }
            else
            {

                for (int i = 1; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }
            }
            dataGridView1.Rows.Add(filas2);

            // mostrar clientes del sistema  Como es la primera iteracion no vamos a tener
            var cantidades = listaDeVectores[0].clientes.Count * 3;
            var filas3 = new string[cantidades + 1];
            filas3[0] = iteracion.ToString();
            var fil1 = 1;
            var fil2 = 2;
            var fil3 = 3;
            for (int i = 0; i < listaDeVectores[0].clientes.Count; i++)
            {
                for (int g = 0; g < listaDeVectores[0].clientes.Count; g++)
                {
                    if ((listaDeVectores[0].clientes[g].estado) == true)
                    {
                        filas3[fil1] = "EnBarra";
                        fil1 += 3;
                    }
                    else
                    {
                        filas3[fil1] = "Atendido";
                        fil1 += 3;
                    }

                    filas3[fil2] = listaDeVectores[0].clientes[g].HorarioLlegada.ToString();
                    filas3[fil3] = listaDeVectores[0].clientes[g].CantidadCliente.ToString();
                    fil3 += 3;
                    fil2 += 3;

                }
                fil1 = 1;
                fil2 = 2;
                fil3 = 3;
            }
            DgvClientes.Rows.Add(filas3);
        }
        
        private void cargarGrillasLlegadaCliente1(List<VectorEstado> listaDeVectores, Queue<Cliente> ColaEnBarra)
        {
            var filass = new string[27];
            filass[0] = iteracion.ToString();
            filass[1] = listaDeVectores[0].nombreEvento;
            filass[2] = listaDeVectores[0].reloj.ToString();
            filass[3] = listaDeVectores[0].llegadaCliente.rndLlegada.ToString();
            filass[4] = listaDeVectores[0].llegadaCliente.tiempoLlegada.ToString();
            filass[5] = listaDeVectores[0].llegadaCliente.proximaLlegada.ToString();
            filass[7] = "";
            
            for (int i = 0; i < 27; i++)
            {
                // printeamos los estados de las mesas
                if (i > 20)
                {
                    if (mesas[i - 21].estado == false)
                    {
                        filass[i] = "Libre";
                    }
                    else if (mesas[i - 21].estado == true)
                    {
                        filass[i] = "Ocupado";
                    }
                }

            }
            DgvPrincipal.Rows.Add(filass);

            //printeeamos el estado de la barra como es la primera iteracion esta todo libre
            var filas2 = new string[16];
            filas2[0] = iteracion.ToString();
            if (ColaEnBarra.Count != 0)
            {
                int lugaresOcupadosEnBarra = 0;
                foreach (var cliente in ColaEnBarra)
                {
                    lugaresOcupadosEnBarra += cliente.CantidadCliente;
                }
                for (int i = 1; i < ColaEnBarra.Count; i++)
                {
                    filas2[i] = "Ocupado";
                }
                for (int i = ColaEnBarra.Count; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }

            }
            else
            {

                for (int i = 1; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }
            }
            dataGridView1.Rows.Add(filas2);

            // mostrar clientes del sistema  Como es la primera iteracion no vamos a tener
            var cantidades = listaDeVectores[0].clientes.Count * 3;
            var filas3 = new string[cantidades + 1];
            filas3[0] = iteracion.ToString();
            var fil1 = 1;
            var fil2 = 2;
            var fil3 = 3;
            for (int i = 0; i < listaDeVectores[0].clientes.Count; i++)
            {
                for (int g = 0; g < listaDeVectores[0].clientes.Count; g++)
                {
                    if ((listaDeVectores[0].clientes[g].estado) == true)
                    {
                        filas3[fil1] = "EnBarra";
                        fil1 += 3;
                    }
                    else
                    {
                        filas3[fil1] = "Atendido";
                        fil1 += 3;
                    }

                    filas3[fil2] = listaDeVectores[0].clientes[g].HorarioLlegada.ToString();
                    filas3[fil3] = listaDeVectores[0].clientes[g].CantidadCliente.ToString();
                    fil3 += 3;
                    fil2 += 3;

                }
                fil1 = 1;
                fil2 = 2;
                fil3 = 3;
            }
            DgvClientes.Rows.Add(filas3);
        }

        private void cargarGrillasBloqueo(List<VectorEstado> listaDeVectores, bool esBloqueoLlegada)
        {
            var filass = new string[27];
            filass[0] = iteracion.ToString();
            filass[1] = listaDeVectores[0].nombreEvento;
            filass[2] = listaDeVectores[0].reloj.ToString();
            filass[5] = listaDeVectores[0].llegadaCliente.proximaLlegada.ToString();
            if (esBloqueoLlegada)
            {
                filass[8] = listaDeVectores[0].proximoBloqueo.RndTipo.ToString();
                filass[9] = "Bloqueo llegada";
                filass[10] = listaDeVectores[0].proximoBloqueo.finBloqueoLlegadaCliente.tiempoBloqueoParcial.ToString();
                filass[11] = listaDeVectores[0].proximoBloqueo.finBloqueoLlegadaCliente.tiempoBloqueoLlegadaCliente.ToString();
            }
            else
            {

                filass[8] = listaDeVectores[0].proximoBloqueo.RndTipo.ToString();
                filass[9] = "Bloqueo barra";
                filass[10] = listaDeVectores[0].proximoBloqueo.finBloqueoBarra.tiempoBloqueoParcial.ToString();
                filass[11] = listaDeVectores[0].proximoBloqueo.finBloqueoBarra.tiempoFinBloqueoMesa.ToString();
            }
            for (int i = 0; i < 27; i++)
            {
                for (int g = 0; g < listaFinAtencion.Count; g++)
                {
                    if (listaDeVectores[0].finDeAtenciones[g].tiempoFinAtencion >= 10000)
                    {
                        filass[15 + g] = "";
                    }
                    else
                    {
                        filass[15 + g] = listaDeVectores[0].finDeAtenciones[g].tiempoFinAtencion.ToString();
                    }
                }

                // printeamos los estados de las mesas
                if (i > 20)
                {
                    if (mesas[i - 21].estado == false)
                    {
                        filass[i] = "Libre";
                    }
                    else if (mesas[i - 21].estado == true)
                    {
                        filass[i] = "Ocupado";
                    }
                }

            }
            DgvPrincipal.Rows.Add(filass);
        }

        private void cargarGrillasLlegadaCliente(List<VectorEstado> listaDeVectores, Queue<Cliente> ColaEnBarra, int calculTiempo)
        {
            var filass = new string[27];
            filass[0] = iteracion.ToString();
            filass[1] = listaDeVectores[0].nombreEvento;
            filass[2] = listaDeVectores[0].reloj.ToString();
            filass[3] = listaDeVectores[0].llegadaCliente.rndLlegada.ToString();
            filass[4] = listaDeVectores[0].llegadaCliente.tiempoLlegada.ToString();
            filass[5] = listaDeVectores[0].llegadaCliente.proximaLlegada.ToString();
            filass[7] = listaDeVectores[0].proximoBloqueo.TiempoProximBloqueo.ToString();
            filass[12] = listaDeVectores[0].llegadaCliente.cliente.rndCantClientes.ToString();
            filass[13] = listaDeVectores[0].llegadaCliente.cliente.CantidadCliente.ToString();

            filass[14] = calculTiempo.ToString();


            for (int i = 0; i < 27; i++)
            {

                for (int g = 0; g < listaFinAtencion.Count; g++)
                {
                    if (listaDeVectores[0].finDeAtenciones[g].tiempoFinAtencion >= 10000)
                    {
                        filass[15 + g] = "";
                    }
                    else
                    {
                        filass[15 + g] = listaDeVectores[0].finDeAtenciones[g].tiempoFinAtencion.ToString();
                    }
                }

                // printeamos los estados de las mesas
                if (i > 20)
                {
                    if (mesas[i - 21].estado == false)
                    {
                        filass[i] = "Libre";
                    }
                    else if (mesas[i - 21].estado == true)
                    {
                        filass[i] = "Ocupado";
                    }
                }

            }
            DgvPrincipal.Rows.Add(filass);

            //mostrar el estado de la barra
            var filas2 = new string[16];
            filas2[0] = iteracion.ToString();
            
            if (ColaEnBarra.Count != 0)
            {
                int lugaresOcupadosEnBarra = 0;
                foreach (var cliente in ColaEnBarra)
                {
                    lugaresOcupadosEnBarra += cliente.CantidadCliente;
                }
                for (int i = 0; i < lugaresOcupadosEnBarra; i++)
                {
                    filas2[i+1] = "Ocupado";
                }
                for (int i = lugaresOcupadosEnBarra + 1; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }

            }
            else
            {

                for (int i = 1; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }
            }
            dataGridView1.Rows.Add(filas2);

            // mostrar clientes del sistema 

            var cantidades = listaDeVectores[0].clientes.Count * 3;
            var filas3 = new string[cantidades + 1];
            filas3[0] = iteracion.ToString();
            var fil1 = 1;
            var fil2 = 2;
            var fil3 = 3;
            for (int i = 0; i < listaDeVectores[0].clientes.Count; i++)
            {
                
                for (int g = 0; g < listaDeVectores[0].clientes.Count; g++)
                {
                    
                    if ((listaDeVectores[0].clientes[g].estado) == true)
                    {
                        filas3[fil1] = "EnBarra";
                        fil1 += 3;
                    }
                    else
                    {
                        filas3[fil1] = "Atendido";
                        fil1 += 3;
                    }

                    filas3[fil2] = listaDeVectores[0].clientes[g].HorarioLlegada.ToString();
                    filas3[fil3] = listaDeVectores[0].clientes[g].CantidadCliente.ToString();
                    fil3 += 3;
                    fil2 += 3;

                }
                fil1 = 1;
                fil2 = 2;
                fil3 = 3;
            }
            DgvClientes.Rows.Add(filas3);
        }

        private void cargarGrillasFinAtencion(List<VectorEstado> listaDeVectores, Queue<Cliente> ColaEnBarra)
        {
            var filass = new string[27];
            filass[0] = iteracion.ToString();
            filass[1] = listaDeVectores[0].nombreEvento;
            filass[2] = listaDeVectores[0].reloj.ToString();
            filass[5] = listaDeVectores[0].llegadaCliente.proximaLlegada.ToString();
            filass[7] = listaDeVectores[0].proximoBloqueo.TiempoProximBloqueo.ToString();
            for (int i = 0; i < 27; i++)
            {
                
                for (int g = 0; g < listaFinAtencion.Count; g++)
                {
                    if (listaDeVectores[0].finDeAtenciones[g].cliente == null || listaDeVectores[0].finDeAtenciones[g].tiempoFinAtencion >= 10000)
                    {
                        filass[15 + g] = "";
                    }
                    else
                    {
                        filass[15 + g] = listaDeVectores[0].finDeAtenciones[g].tiempoFinAtencion.ToString();
                    }

                }

                // printeamos los estados de las mesas
                if (i > 20)
                {
                    if (mesas[i - 21].estado == false)
                    {
                        filass[i] = "Libre";
                    }
                    else if (mesas[i - 21].estado == true)
                    {
                        filass[i] = "Ocupado";
                    }
                }

            }
            DgvPrincipal.Rows.Add(filass);

            //mostrar el estado de la barra
            var filas2 = new string[16];
            filas2[0] = iteracion.ToString();
            if (ColaEnBarra.Count != 0)
            {
                int lugaresOcupadosEnBarra = 0;
                foreach (var cliente in ColaEnBarra)
                {
                    lugaresOcupadosEnBarra += cliente.CantidadCliente;
                }
                for (int i = 1; i < lugaresOcupadosEnBarra; i++)
                {
                    filas2[i] = "Ocupado";
                }
                for (int i = lugaresOcupadosEnBarra; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }

            }
            else
            {

                for (int i = 1; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }
            }
            dataGridView1.Rows.Add(filas2);

            // mostrar clientes del sistema 
            var cantidades = listaDeVectores[0].clientes.Count * 3;
            var filas3 = new string[cantidades + 1];
            filas3[0] = iteracion.ToString();
            var fil1 = 1;
            var fil2 = 2;
            var fil3 = 3;
            for (int i = 0; i < listaDeVectores[0].clientes.Count; i++)
            {
                for (int g = 0; g < listaDeVectores[0].clientes.Count; g++)
                {
                    if ((listaDeVectores[0].clientes[g].estado) == true)
                    {
                        filas3[fil1] = "EnBarra";
                        fil1 += 3;
                    }
                    else
                    {
                        filas3[fil1] = "Atendido";
                        fil1 += 3;
                    }

                    filas3[fil2] = listaDeVectores[0].clientes[g].HorarioLlegada.ToString();
                    filas3[fil3] = listaDeVectores[0].clientes[g].CantidadCliente.ToString();
                    fil3 += 3;
                    fil2 += 3;

                }
                fil1 = 1;
                fil2 = 2;
                fil3 = 3;
            }
            DgvClientes.Rows.Add(filas3);
        }

        private void cargarGrillas(List<VectorEstado> listaDeVectores, Queue<Cliente> ColaEnBarra)
        {
            var filass = new string[27];
            filass[0] = iteracion.ToString();
            filass[1] = listaDeVectores[0].nombreEvento;
            filass[2] = listaDeVectores[0].reloj.ToString();
            filass[3] = listaDeVectores[0].llegadaCliente.rndLlegada.ToString();
            filass[4] = listaDeVectores[0].llegadaCliente.tiempoLlegada.ToString();
            filass[5] = listaDeVectores[0].llegadaCliente.proximaLlegada.ToString();
            filass[6] = listaDeVectores[0].proximoBloqueo.TBloqueo.ToString();
            filass[7] = listaDeVectores[0].proximoBloqueo.TiempoProximBloqueo.ToString();

            for (int i = 0; i < 27; i++)
            {
                // printeamos los estados de las mesas
                if (i > 20)
                {
                    if (mesas[i - 21].estado == false)
                    {
                        filass[i] = "Libre";
                    }
                    else if (mesas[i - 21].estado == true)
                    {
                        filass[i] = "Ocupado";
                    }
                }

            }
            DgvPrincipal.Rows.Add(filass);

            //printeeamos el estado de la barra como es la primera iteracion esta todo libre
            var filas2 = new string[16];
            filas2[0] = iteracion.ToString();
            if (ColaEnBarra.Count != 0)
            {
                int lugaresOcupadosEnBarra = 0;
                foreach (var cliente in ColaEnBarra)
                {
                    lugaresOcupadosEnBarra += cliente.CantidadCliente;
                }
                for (int i = 1; i < lugaresOcupadosEnBarra; i++)
                {
                    filas2[i] = "Ocupado";
                }
                for (int i = lugaresOcupadosEnBarra; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }

            }
            else
            {

                for (int i = 1; i < 16; i++)
                {
                    filas2[i] = "Libre";
                }
            }
            dataGridView1.Rows.Add(filas2);



            // mostrar clientes del sistema  Como es la primera iteracion no vamos a tener
            var cantidades = listaDeVectores[0].clientes.Count * 3;
            var filas3 = new string[cantidades + 1];
            filas3[0] = iteracion.ToString();
            var fil1 = 1;
            var fil2 = 2;
            var fil3 = 3;
            for (int i = 0; i < listaDeVectores[0].clientes.Count; i++)
            {
                for (int g = 0; g < listaDeVectores[0].clientes.Count; g++)
                {
                    if ((listaDeVectores[0].clientes[g].estado) == true)
                    {
                        filas3[fil1] = "EnBarra";
                        fil1 += 3;
                    }
                    else
                    {
                        filas3[fil1] = "Atendido";
                        fil1 += 3;
                    }

                    filas3[fil2] = listaDeVectores[0].clientes[g].HorarioLlegada.ToString();
                    filas3[fil3] = listaDeVectores[0].clientes[g].CantidadCliente.ToString();
                    fil3 += 3;
                    fil2 += 3;

                }
                fil1 = 1;
                fil2 = 2;
                fil3 = 3;
            }
            DgvClientes.Rows.Add(filas3);
        }

        private void TxtTiempoSimulacion_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            {
                MessageBox.Show("¡Solo se permite ingresar números enteros!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var frmRK4O  = new RK4O();
            frmRK4O.ShowDialog();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DgvPrincipal_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.DgvPrincipal.Columns[e.ColumnIndex].Name == "Reloj")
            {
                e.CellStyle.BackColor = Color.Pink;
                e.CellStyle.ForeColor = Color.Green;
            }
            if (this.DgvPrincipal.Columns[e.ColumnIndex].Name == "NombreEvento" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "TipoAtentado")
            {
                //e.CellStyle.Font = "Century Gothic";
            }

            if (this.DgvPrincipal.Columns[e.ColumnIndex].Name == "Mesa1" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "Mesa2" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "Mesa3" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "Mesa4" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "Mesa5" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "Mesa6")
            {
                if (Convert.ToString(e.Value) == "Ocupado")
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.BackColor = Color.White;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.Green;
                    e.CellStyle.BackColor = Color.White;
                }

            }
            if (this.DgvPrincipal.Columns[e.ColumnIndex].Name == "ProximaLlegada"
                || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "LlegadaProximoAtentado"
                || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "tfinbloqueo"
                || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "FinAtencion1"
                || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "finAtencion2" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "finAtencion3" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "FinAtencion4" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "FinAtencion5" || this.DgvPrincipal.Columns[e.ColumnIndex].Name == "FinAtencion6"
                )
            {
                e.CellStyle.BackColor = Color.LightGoldenrodYellow;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Column1" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Column3" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Column5" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Column7" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Column9" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado6" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado7" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado8" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado9" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado10" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado11" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado12" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado13" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado14" || this.dataGridView1.Columns[e.ColumnIndex].Name == "Estado15")
            {
                if (Convert.ToString(e.Value) == "Ocupado")
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.BackColor = Color.White;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.Green;
                    e.CellStyle.BackColor = Color.White;
                }

            }
        }

        private void DgvClientes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            
                if (Convert.ToString(e.Value) == "Atendido")
                {
                    e.CellStyle.ForeColor = Color.Blue;
                    e.CellStyle.BackColor = Color.LightGray;
                }
                else if (Convert.ToString(e.Value) == "EnBarra")
                {
                    e.CellStyle.ForeColor = Color.Yellow;
                    e.CellStyle.BackColor = Color.LightGray;
                }

            

        }
    }
}