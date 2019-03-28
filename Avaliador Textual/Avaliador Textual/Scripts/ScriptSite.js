// Utilizado na página de Avaliação de Textos
function openTab(evt, tabId) {
    var i, x, tablinks;
    x = document.getElementsByClassName("opcaoEntrada");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablink");
    for (i = 0; i < x.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" tabMarcada", "");
    }
    document.getElementById(tabId).style.display = "block";
    evt.currentTarget.className += " tabMarcada";

    document.getElementById("Texto").value = '';
    document.getElementById("Nome").value = '';
}


// Dropzone
Dropzone.options.myDropzone = {
    autoProcessQueue: false, //prevents Dropzone from uploading dropped files immediately
    maxFilesize: 20,
    uploadMultiple: true,
    parallelUploads: 10000,
    acceptedFiles: ".txt",
    dictDefaultMessage: "Arraste arquivos ou clique aqui.",
    dictInvalidFileType: "Arquivo de extensão inválida.",
    dictFileTooBig: "Arquivo muito grande.",
    dictMaxFilesExceeded: "Máximo de arquivos atingido.",
    init: function () {
        var myDropzone = this;

        myDropzone.on("addedfile", function (file) {
            document.querySelector("#nroArquivos").innerHTML =
                'Quantidade de arquivos selecionados: ' +
                '<div style="display: inline-block;font-size:20px; color:red;">' +
                    myDropzone.files.length +
                '</div>';

            file.previewElement.addEventListener("click", function () {
                myDropzone.removeFile(file);
            });
        });

        myDropzone.on("removedfile", function (file) {
            document.querySelector("#nroArquivos").innerHTML =
                'Quantidade de arquivos selecionados: ' +
                '<div style="display: inline-block;font-size:20px; color:red;">' +
                    myDropzone.files.length +
                '</div>';
        });

        var submitButton = document.querySelector("#btnUpload");
        submitButton.addEventListener("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            //tell Dropzone to process all queued files
            myDropzone.processQueue();
        });

        myDropzone.on("queuecomplete", function () {
            if (myDropzone.getUploadingFiles().length === 0 && myDropzone.getQueuedFiles().length === 0) {
                if (myDropzone.getRejectedFiles().length === 0) {
                    var url = '/Arquivos/Index';
                    location.href = url;
                }
            }
        });

    }
};


//DataTable
    $(document).ready(function () {
        var table = $('#myTable').DataTable({
            "paging": true,
            "pagingType": "full_numbers",
            "pageLength": 10,
            "bProcessing": true,
            "bDeferRender": true,
            "bLengthChange": false,
            "language": {
                "sEmptyTable": "Nenhum registro encontrado",
                "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
                "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
                "sInfoFiltered": "(Filtrados de _MAX_ registros)",
                "sInfoPostFix": "",
                "sInfoThousands": ".",
                "sLengthMenu": "_MENU_ resultados por página",
                "sLoadingRecords": "Carregando...",
                "sProcessing": "Processando...",
                "sZeroRecords": "Nenhum registro encontrado",
                "sSearch": "Pesquisar",
                "oPaginate": {
                    "sNext": "Próximo",
                    "sPrevious": "Anterior",
                    "sFirst": "<<",
                    "sLast": ">>"
                },
                "oAria": {
                    "sSortAscending": ": Ordenar colunas de forma ascendente",
                    "sSortDescending": ": Ordenar colunas de forma descendente"
                }
            },
            "sDom": 't<"clearfix"p>'
        });

        $('#searchFilter').on('keyup', function () {
            table.search(this.value).draw();
        });
    });



$(document).ready(function () {

    $('.toggle-view').click(function () {
        var text = $(this).children('div');

        if (text.is(':hidden')) {
            text.slideDown('200');
            $(this).children('span').html('-');
        } else {
            text.slideUp('200');
            $(this).children('span').html('+');
        }
    });

});