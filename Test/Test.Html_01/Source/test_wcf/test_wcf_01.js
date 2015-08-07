function Clear() {
    $('#jsonData').html('');
}

function Test1() {
    $('#jsonData').html('');
    $('#jsonData').append("toto");
}

function Test2() {
    $('#jsonData').html('');
    $('#jsonData').append("tata");
}

function Test3() {
    $('#jsonData').html('');
    $('#jsonData').append('<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/">You entered: 123</string>');
}

function LoadUrl(zzurl) {
    $.ajax({
        type: "GET",
        // http://localhost:4545/Service/Service1.svc/GetJson
        // http://localhost:8803/test_wcf_03/Service01.svc
        //url: "http://localhost:8803/test_wcf_03/Service01.svc/GetJson",
        //url: "http://localhost:8803/test_wcf_03/Service01/GetJson",
        //url: "/RestTestService.svc/GetString_Get",
        //url: "http://localhost:11937/RestTestService.svc/GetString_Get",
        url: zzurl,
        contentType: "application/json;charset=utf-8",
        //dataType: "json",
        success: function (data) {
            //var result = data.GetEmployeeJSONResult;
            //var id = result.Id;
            //var name = result.Name;
            //var salary = result.Salary;
            $('#jsonData').html('');
            //$('#jsonData').append('<table border="1"><tbody><tr><th>' +
            //  "Employee Id</th><th>Name</th><th>Salary</th>" +
            //  '</tr><tr><td>' + id + '</td><td>' + name +
            //  '</td><td>' + salary + '</td></tr></tbody></table>');
            $('#jsonData').append(data);
        },
		// error: Function( jqXHR jqXHR, String textStatus, String errorThrown )  from http://api.jquery.com/jquery.ajax/
        error: function (jqXHR, textStatus, errorThrown) {
            $('#jsonData').html('');
            $('#jsonData').append('error');
            $('#jsonData').append('<br/>');
            $('#jsonData').append(textStatus);
            $('#jsonData').append('<br/>');
            $('#jsonData').append(errorThrown);
            $('#jsonData').append('<br/>');
            //alert(xhr.responseText);
        }
    });
}
