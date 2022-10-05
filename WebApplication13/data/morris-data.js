$(function() {

    Morris.Area({
        element: 'morris-area-chart',
        data: [
          { datetime: '2016-12-20 06:00:00', Sale: 0 },
          { datetime: '2016-12-20 07:00:00', Sale: 0 },
          { datetime: '2016-12-20 08:00:00', Sale: 0 },
          { datetime: '2016-12-20 09:00:00', Sale: 0 },
          { datetime: '2016-12-20 10:00:00', Sale: 2 },
          { datetime: '2016-12-20 11:00:00', Sale: 4 },
          { datetime: '2016-12-20 12:00:00', Sale: 1 },
          { datetime: '2016-12-20 13:00:00', Sale: 3 },
          { datetime: '2016-12-20 14:00:00', Sale: 5 },
          { datetime: '2016-12-20 15:00:00', Sale: 10 },
          { datetime: '2016-12-20 16:00:00', Sale: 1 },
          { datetime: '2016-12-20 17:00:00', Sale: 18 },
          { datetime: '2016-12-20 18:00:00', Sale: 4 },
          { datetime: '2016-12-20 19:00:00', Sale: 1 },
          { datetime: '2016-12-20 20:00:00', Sale: 0 },
          { datetime: '2016-12-20 21:00:00', Sale: 0 },
          { datetime: '2016-12-20 22:00:00', Sale: 5 },
          { datetime: '2016-12-20 23:00:00', Sale: 6 },
          { datetime: '2016-12-20 24:00:00', Sale: 7 },
          { datetime: '2016-12-20 01:00:00', Sale: 1 },
          { datetime: '2016-12-20 02:00:00', Sale: 0 },
          { datetime: '2016-12-20 03:00:00', Sale: 0 },
          { datetime: '2016-12-20 04:00:00', Sale: 0 },
          { datetime: '2016-12-20 05:00:00', Sale: 0 }
        ],
        xkey: 'datetime',
        ykeys: ['Sale'],
        labels: ['Sale'],
        pointSize: 2,
        hideHover: 'auto',
        resize: true
    });
    /*
    Morris.Area({
        element: 'morris-area-chartss',
        data: [
          { datetime: '2016-12-20 06:00:00', Sale: 0 },
          { datetime: '2016-12-20 07:00:00', Sale: 0 },
          { datetime: '2016-12-20 08:00:00', Sale: 0 },
          { datetime: '2016-12-20 09:00:00', Sale: 0 },
          { datetime: '2016-12-20 10:00:00', Sale: 2 },
          { datetime: '2016-12-20 11:00:00', Sale: 4 },
          { datetime: '2016-12-20 12:00:00', Sale: 1 },
          { datetime: '2016-12-20 13:00:00', Sale: 3 },
          { datetime: '2016-12-20 14:00:00', Sale: 5 },
          { datetime: '2016-12-20 15:00:00', Sale: 10 },
          { datetime: '2016-12-20 16:00:00', Sale: 1 },
          { datetime: '2016-12-20 17:00:00', Sale: 18 },
          { datetime: '2016-12-20 18:00:00', Sale: 4 },
          { datetime: '2016-12-20 19:00:00', Sale: 1 },
          { datetime: '2016-12-20 20:00:00', Sale: 0 },
          { datetime: '2016-12-20 21:00:00', Sale: 0 },
          { datetime: '2016-12-20 22:00:00', Sale: 5 },
          { datetime: '2016-12-20 23:00:00', Sale: 6 },
          { datetime: '2016-12-20 24:00:00', Sale: 7 },
          { datetime: '2016-12-20 01:00:00', Sale: 1 },
          { datetime: '2016-12-20 02:00:00', Sale: 0 },
          { datetime: '2016-12-20 03:00:00', Sale: 0 },
          { datetime: '2016-12-20 04:00:00', Sale: 0 },
          { datetime: '2016-12-20 05:00:00', Sale: 0 }
        ],
        xkey: 'datetime',
        ykeys: ['Sale'],
        labels: ['Sale'],
        pointSize: 2,
        hideHover: 'auto',
        resize: true
    });
    */
    Morris.Donut({
        element: 'morris-donut-chart',
        data: [{
            label: "Download Sales",
            value: 12
        }, {
            label: "In-Store Sales",
            value: 30
        }, {
            label: "Mail-Order Sales",
            value: 20
        }],
        resize: true
    });

    Morris.Bar({
        element: 'morris-bar-chart',
        data: [{
            y: '2006',
            a: 100,
            b: 90
        }, {
            y: '2007',
            a: 75,
            b: 65
        }, {
            y: '2008',
            a: 50,
            b: 40
        }, {
            y: '2009',
            a: 75,
            b: 65
        }, {
            y: '2010',
            a: 50,
            b: 40
        }, {
            y: '2011',
            a: 75,
            b: 65
        }, {
            y: '2012',
            a: 100,
            b: 90
        }],
        xkey: 'y',
        ykeys: ['a', 'b'],
        labels: ['Series A', 'Series B'],
        hideHover: 'auto',
        resize: true
    });

    Morris.Area({
        element: 'graph',
        data: [
          { x: '2010 Q4', y: 3, z: 7 },
          { x: '2011 Q1', y: 3, z: 4 },
          { x: '2011 Q2', y: null, z: 1 },
          { x: '2011 Q3', y: 2, z: 5 },
          { x: '2011 Q4', y: 8, z: 2 },
          { x: '2012 Q1', y: 4, z: 4 }
        ],
        xkey: 'x',
        ykeys: ['y', 'z'],
        labels: ['Y', 'Z']
    }).on('click', function (i, row) {
        console.log(i, row);
    });
    
});
