// Example starter JavaScript for disabling form submissions if there are invalid fields
(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                    form.classList.remove('was-validated');
                }
                form.classList.add('was-validated');
            }, false);
        });

    }, false);
})();
$(document).ready(function () {
    var forms = document.getElementsByClassName('needs-validation');
    var validation = Array.prototype.filter.call(forms, function (form) {


        $('#submit_edit_product').on('click', function () {
            if (form.checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                form.classList.remove('was-validated');

            } else {
                Update_Product();
            }
            form.classList.add('was-validated');
        })
        $('#submit_reason').on('click', function () {
            if (form.checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                form.classList.remove('was-validated');
            } else {
                DeleteOrder();
            }
            form.classList.add('was-validated');
        })


    }, false);
})
$('.CGForm').submit(function (e) {
    var form = $(this);

    // Check if form is valid then submit ajax
    if (form[0].checkValidity()) {
        e.preventDefault();
        var url = form.attr('action');
        $.ajax({
            url: url,
            type: 'POST',
            data: form.serialize(),
            success: function (data) {
                // Hide bootstrap modal to prevent conflict
                $('.modal').modal('hide');

                if (data.status) {
                    // Refresh table data
                    GetList_CategoryAndType();
                    sweetAlert
                        ({
                            title: "Success !",
                            text: data.message,
                            type: "success"
                        })

                    form[0].reset();
                    form.removeClass('was-validated');
                } else {
                    swal({
                        title: 'Error !',
                        text: data.message,
                        type: 'error',

                    }); // Show bootstrap modal again
                }
            }
        });
    }
});

//-------------------------Delete Category, Product-----------------------
var URLDelete = "";
$('#URLDelete')
    .keypress(function () {
        URLDelete = $(this).val();
    })
    .keypress();

function Contains(text_one, text_two) {
    if (text_one.indexOf(text_two) != -1)
        return true;
}
function deleteAlert(id, code) {
    swal({
        title: "Are you sure you want to delete ?",
        type: "warning",
        showCancelButton: true,
        closeOnConfirm: false,
        confirmButtonText: "Yes",
        confirmButtonColor: "#ec6c62",
    },
        function () {
            var categorys = {};
            categorys.ID = id;

            $.ajax({
                url: URLDelete,
                data: JSON.stringify(categorys),
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json'
            })
                .done(function (data) {
                    if (data.status == true) {
                        sweetAlert
                            ({
                                title: "Delete successfully",
                                confirmButtonText: "OK",
                                type: "success"
                            },
                                function () {
                                    $("table tbody").find('input[name="record"]').each(function () {
                                        if (Contains($(this).val(), code)) {
                                            $(this).parents("tr").remove();
                                        }
                                    });
                                })
                    }
                    else {
                        sweetAlert
                            ({
                                title: "Delete failed !",
                                confirmButtonText: "OK",
                                type: "error"
                            })
                    }

                })
            /*   .error(function (data) {
                   swal("OOps", "Chúng tôi không thể kết nối đến server!", "error");
               })*/
        })
}

//-----------------------Edit status------------------------------------------
var URDEditStatus = "";
$('#URDEditStatus')
    .keypress(function () {
        URDEditStatus = $(this).val();
    })
    .keypress();
function EditStatus(id) {
    console.log(URDEditStatus);
    sweetAlert
        ({
            title: "Status update successful!",
            confirmButtonText: "OK",
            type: "success"
        },
            function () {
                var categorys = {};
                categorys.id = id;
                $.ajax({
                    url: URDEditStatus,
                    data: JSON.stringify(categorys),
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json'
                })
            });

}

//-----------------------Định dạng giá-------------------------------------
$('.productPrice').keydown(function (e) {
    setTimeout(() => {
        let parts = $(this).val().split(".");
        let v = parts[0].replace(/\D/g, ""),
            dec = parts[1]
        let calc_num = Number((dec !== undefined ? v + "." + dec : v));
        // use this for numeric calculations
        // console.log('number for calculations: ', calc_num);
        let n = new Intl.NumberFormat('en-EN').format(v);
        n = dec !== undefined ? n + "." + dec : n;
        $(this).val(n);
    })
})

$('#edit_product_price').keydown(function (e) {
    setTimeout(() => {
        let parts = $(this).val().split(".");
        let v = parts[0].replace(/\D/g, ""),
            dec = parts[1]
        let calc_num = Number((dec !== undefined ? v + "." + dec : v));
        // use this for numeric calculations
        // console.log('number for calculations: ', calc_num);
        let n = new Intl.NumberFormat('en-EN').format(v);
        n = dec !== undefined ? n + "." + dec : n;
        $(this).val(n);
    })
})




//-------------------------Get Category -------------------------------

$('#URLFindCategory')
    .keypress(function () {
        URLFindCategory = $(this).val();
    })
    .keypress();
function GetCategory(ele, id) {
    row = $(ele).closest('tr');
    $.ajax({
        type: 'POST',
        url: URLFindCategory,
        data: { "category_id": id },
        success: function (response) {
            $('#edit_id').val(response.ID);
            $('#Edit_name').val(response.Name);

            $('#Edit_Modal .close').css('display', 'none');
            $('#Edit_Modal').modal('show');
        }
    })
}

//------------------------UPDATE CATEGORY-----------------------------

$('#URL_List')
    .keypress(function () {
        URL_List = $(this).val();
    })
    .keypress();


function GetList_CategoryAndType() {
    $.ajax({
        url: URL_List,
        data: {

        }
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable()

    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}
//------------ Load dropdown form add product----------------------------------

var URLgetCategory = "";
var URLgetType = "";

$('#URLgetCategory')
    .keypress(function () {
        URLgetCategory = $(this).val();
    })
    .keypress();
$('#URLgetType')
    .keypress(function () {
        URLgetType = $(this).val();
    })
    .keypress();


$.ajax({
    type: "GET",
    url: URLgetCategory,
    data: "{}",
    success: function (data) {
        var edit_Category_id = $('#edit_Category_id').val();
        if (edit_Category_id != undefined) {
            var s = '<option value="" disabled="disabled">Select product category</option>';
        } else {
            var s = '<option value="" disabled="disabled" selected="selected">Select product category</option>';

        }
        for (var i = 0; i < data.length; i++) {
            if (edit_Category_id == data[i].categoryID) {
                s += '<option value="' + data[i].categoryID + '" selected="selected">' + data[i].categoryName + '</option>';

            } else {
                s += '<option value="' + data[i].categoryID + '">' + data[i].categoryName + '</option>';
            }
        }
        $("#CategoryDropdown").html(s);
    }
});
$.ajax({
    type: "GET",
    url: URLgetType,
    data: "{}",
    success: function (data) {
        var edit_Type_id = $('#edit_Type_id').val();
        if (edit_Type_id != undefined) {
            var s = '<option value="" disabled="disabled">Select product type</option>';
        } else {
            var s = '<option value="" disabled="disabled" selected="selected">Select product type</option>';

        }
        for (var i = 0; i < data.length; i++) {
            if (edit_Type_id == data[i].TypeID) {
                s += '<option value="' + data[i].TypeID + '" selected="selected">' + data[i].TypeName + '</option>';
            } else {
                s += '<option value="' + data[i].TypeID + '">' + data[i].TypeName + '</option>';
            }
        }
        $("#TypeDropdown").html(s);
    }
});
$.ajax({
    type: "GET",
    url: URLgetCategory,
    data: "{}",
    success: function (data) {
        var s = '<option value="-1" >Select product category</option>';
        for (var i = 0; i < data.length; i++) {
            s += '<option value="' + data[i].categoryID + '">' + data[i].categoryName + '</option>';
        }
        $("#filter_Category").html(s);
    }
});
$.ajax({
    type: "GET",
    url: URLgetType,
    data: "{}",
    success: function (data) {
        var s = '<option value="-1" >Select product type</option>';
        for (var i = 0; i < data.length; i++) {
            s += '<option value="' + data[i].TypeID + '">' + data[i].TypeName + '</option>';
        }
        $("#filter_Type").html(s);
    }
});

//----------------------FILTER PRODUCT------------------------------------------------
$('#URLProductList')
    .keypress(function () {
        URLProductList = $(this).val();
    })
    .keypress();


$("#filter_Category").change(function () {
    var type_id = $("#filter_Type").val();
    var category_id = $("#filter_Category").val();
    GetList(type_id, category_id)
});
$("#filter_Type").change(function () {
    var type_id = $("#filter_Type").val();
    var category_id = $("#filter_Category").val();
    GetList(type_id, category_id)
});

function GetList(type_id, category_id) {
    $.ajax({
        url: URLProductList,
        data: {
            category_id: category_id,
            type_id: type_id,
        }
    }).done(function (result) {
        $('#dataContainer').html(result);
        $('#example').DataTable()

    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
        console.log(textStatus)
        console.log(errorThrown)
        alert("Something Went Wrong, Try Later");
    });
}

//-----------------------Edit status------------------------------------------

function EditStatus_comment(id, status) {
    var URDEditStatus_Comment = "";
    $('#URDEditStatus_Comment')
        .keypress(function () {
            URDEditStatus_Comment = $(this).val();
        })
        .keypress();
    var cmt = {};
    cmt.id = id;
    cmt.Status = status;
    $.ajax({
        url: URDEditStatus_Comment,
        data: JSON.stringify(cmt),
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            sweetAlert
                ({
                    title: "Status update successful!",
                    confirmButtonText: "OK",
                    type: "success"
                })

            // Refresh admin count of comment
            $("#commentCount").parent().load('/Home/GetAdminSidebar' + " #commentCount");

            GetList_comment()
        },
    });

}
//-------------------------Get Delete Order -------------------------------
function GetOrder(id) {
    console.log(id);
    $('#edit_idOrder').val(id);
    $('#DeleteOrder.close').css('display', 'none');
    $('#DeleteOrder').modal('show');

}
//-----------------------Delete order------------------------------------------

function DeleteOrder() {
    var URDDeleteOrder = "";
    $('#URDDeleteOrder')
        .keypress(function () {
            URDDeleteOrder = $(this).val();
        })
        .keypress();

    var CustomerOrder = {};
    CustomerOrder.id = $('#edit_idOrder').val();
    CustomerOrder.Reason = $('#reason').val();

    $.ajax({
        url: URDDeleteOrder,
        data: JSON.stringify(CustomerOrder),
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            $('#DeleteOrder.close').css('display', 'none');
            $('#DeleteOrder').modal('hide');
            $('.modal ').insertAfter($('body'));
            sweetAlert
                ({
                    title: "Canceled order successfully!",
                    confirmButtonText: "OK",
                    type: "success"
                })
            var date_start = $("#filter_DateStart").val();
            var date_end = $("#filter_DateEnd").val();
            GetList_order(date_start, date_end)
        },
    });

}
//-------------------------Get Delete Order -------------------------------
function GetCmt(id, productID) {
    $('#edit_idComment').val(id);
    $('#edit_idProduct').val(productID);

    $('#ReplyComment.close').css('display', 'none');
    $('#ReplyComment').modal('show');

}

//-----------------------Reply comment------------------------------------------

function Reply_comment() {
    var URDReplyComment = "";
    $('#URDReplyComment')
        .keypress(function () {
            URDReplyComment = $(this).val();
        })
        .keypress();

    var cmt = {};
    cmt.Reply_coment = $('#edit_idComment').val();
    cmt.Content = $('#replyComment').val();
    cmt.ProductID = $('#edit_idProduct').val();

    $.ajax({
        url: URDReplyComment,
        data: JSON.stringify(cmt),
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            $('#ReplyComment.close').css('display', 'none');
            $('#ReplyComment').modal('hide');
            $('.modal ').insertAfter($('body'));
            sweetAlert
                ({
                    title: "Successful reply",
                    confirmButtonText: "OK",
                    type: "success"
                })

            GetList_comment();
        },
    });

}