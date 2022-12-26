/*$(function() {

    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    today = yyyy + '-' + mm + '-' + dd;

    var setNum = [];
    for (var a = 0; a < 24; a++) {
        setNum.push(Math.floor((Math.random() * 100) + 1));
        console.log(setNum[a]);
    }

    function data() {
        var ret = [];
        for (var x = 0; x < 24; x++) {
            var c = setNum[x];
            ret.push({
                x: today + ' ' + x + ':00:00',
                y: c
            });
        }
        return ret;
    }
    var graph = Morris.Area({
        element: 'mygraph',
        data: data(),
        xkey: 'x',
        ykeys: ['y'],
        labels: ['Sale'],
        hideHover: true
    });

});*/