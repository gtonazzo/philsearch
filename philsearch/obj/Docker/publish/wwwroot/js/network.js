
var forceStrenght = 50;
var svg;
var color = d3.scaleOrdinal(d3.schemePastel1);
var node, link;
var d3graph;
var simulation = d3.forceSimulation();

/*
    svg = d3.select("#networkSVG"),
        width = +svg.attr("width"),
        height = +svg.attr("height");
    */
// Set the dimensions and margins of the diagram
var margin = { top: 20, right: 120, bottom: 120, left: 90 },
    width = 1960 - margin.left - margin.right,
    height = 1960 - margin.top - margin.bottom;

function displayGraph(graph) {
    

    // append the svg object to the body of the page
    // appends a 'group' element to 'svg'
    // moves the 'group' element to the top left margin
    var svg = d3.select("#networkSVG")
        .attr("width", width + margin.right + margin.left)
        .attr("height", height + margin.top + margin.bottom);



    link = svg.append("g")
        .attr("class", "links")
        .selectAll("line")
        .data(graph.links)
        .enter().append("line")
        .attr("stroke-width", function (d) { return Math.sqrt(d.value) ; });

    node = svg.append("g")
        .attr("class", "nodes")
        .selectAll("g")
        .data(graph.nodes)
        .enter().append("g")

    var circles = node.append("circle")
        .attr("r", function (d) { return 4 * (parseInt(d.group) + 1); })
        .attr("fill", function (d) { return color(d.group); })
        .call(d3.drag()
            .on("start", dragstarted)
            .on("drag", dragged)
            .on("end", dragended));

    var lables = node.append("text")
        .text(function (d) {
            return d.id;
        })
        .attr("font-size", function (d) { return 10 + (d.group * 2); })
        .attr('x', 0)
        .attr('y', 0)
        .attr("text-anchor", "middle");

    node.append("title")
        .text(function (d) { return d.id; });

    
    d3graph = graph;
    drawGraph();
 
};


function drawGraph() {
    
    simulation
        .force("link", d3.forceLink().id(function (d) { return d.id; }))
        .force("charge", d3.forceManyBody().strength(-forceStrenght))
        .force("center", d3.forceCenter(width / 2, height / 2));

    simulation
        .nodes(d3graph.nodes)
        .on("tick", ticked);

    simulation.force("link")
        .links(d3graph.links);

    function ticked() {
        link
            .attr("x1", function (d) { return d.source.x; })
            .attr("y1", function (d) { return d.source.y; })
            .attr("x2", function (d) { return d.target.x; })
            .attr("y2", function (d) { return d.target.y; });

        node
            .attr("transform", function (d) {
                return "translate(" + d.x + "," + d.y + ")";
            })

    }
}



function dragstarted(d) {
    if (!d3.event.active) simulation.alphaTarget(0.3).restart();
    d.fx = d.x;
    d.fy = d.y;
}

function dragged(d) {
    d.fx = d3.event.x;
    d.fy = d3.event.y;
}

function dragended(d) {
    if (!d3.event.active) simulation.alphaTarget(0);
    d.fx = null;
    d.fy = null;
}