$(document).on("ready",inicio);

//Funcion principal que inicia todo
function inicio() {
	$("#acercaInst").on("click",acercaInst);
	$("#contInst").on("click",contInst);
	$("#evenInst").on("click",evenInst);
	$("#notInst").on("click",notInst);
	$("#trayectInst").on("click",trayectInst);

	function initialize() {
		var myLatlng = new google.maps.LatLng(-27.452031, -58.981001);
		var mapOptions = {
			zoom: 15,
			center: myLatlng
		}
		var map = new google.maps.Map(document.getElementById('map'), mapOptions);

		var marker = new google.maps.Marker({
			position: myLatlng,
			map: map,
			title: 'Universidad Tecnologica Nacional'
		});
	}

	google.maps.event.addDomListener(window, 'load', initialize);

}

//Funcion que muesta info acerca intituciones
function acercaInst(e){
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#acercaDeInst").css("display","block");
}

//Funcion que muestra info contacto instituciones 
function contInst(e) {
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#contactoInst").css("display","block");	
}

//Funcion que muestra info eventos instituciones 
function evenInst(e) {
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#eventosInst").css("display","block");	
}

//Funcion que muestra info noticias instituciones 
function notInst(e) {
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#noticiasInst").css("display","block");	
}

//Funcion que muestra info trayectoria instituciones
function trayectInst(e) {
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#trayectoriaInst").css("display","block");	
}

//Funcion para agregar la clase navActivoInst
function agregarClaseActiva(thiis){
	for (var i = 0; i < 5; i++) {
		$(".navInstituciones ul li a").removeClass("navActivoInst");
	};
	console.log(thiis);
	$(thiis).addClass("navActivoInst");
}

//Funcion para ocultar los div
function ocultarDivs(){
	for (var i = 0; i < 5; i++) {
		$("#textNavInst div").css("display","none");
	};
}