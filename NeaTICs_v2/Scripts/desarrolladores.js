$(document).on("ready",iniciar);

function iniciar(){
	$("#los22Men").on("click",mostrarLos22);
	$("#losImplemMen").on("click",mostrarLosImple);
	$("#maniaMen1").on("click",mostrarMania1);
	$("#maniaMen2").on("click",mostrarMania2);
}

function mostrarLos22(e) {
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#los22SubMen").show("slow");
}

function mostrarLosImple(e) {
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#losImplemSubMen").show("slow");
}

function mostrarMania1(e) {
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#maniaSubMen1").show("slow");
}

function mostrarMania2(e) {
	e.preventDefault();
	agregarClaseActiva(this);
	ocultarDivs();
	$("#maniaSubMen2").show("slow");
}

function agregarClaseActiva(thiis){
	for (var i = 0; i < 5; i++) {
		$("#subMenuDes ul li a").removeClass("activarSubMenu");
	};
	$(thiis).addClass("activarSubMenu");
}

//Funcion para ocultar los div
function ocultarDivs(){
	$("#los22SubMen").css("display","none");
	$("#losImplemSubMen").css("display","none");
	$("#maniaSubMen1").css("display","none");
	$("#maniaSubMen2").css("display","none");
}