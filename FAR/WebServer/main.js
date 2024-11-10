const fs = require('fs')
const express = require('express')
var bodyParser = require('body-parser')
var jsonParser = bodyParser.json()
var urlencodedParser = bodyParser.urlencoded({ extended: false })
const app = express()
const port = 3003

const io = require('socket.io')();
io.on('connection', client => { console.log("Connected!") });
io.listen(port+1);

app.get('/', (req, res) => {
    res.send(`
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Fishi's Anti Recoil</title>
    <script>
    function submitconfig() {
        document.getElementById('configForm').submit();
    }
    function submitdata() {
        document.getElementById('inputForm').submit();
    }
    </script>
</head>
<body>
<iframe name="dummyframe" id="dummyframe" style="display: none;"></iframe>
    <h1>Config Selector</h1>

    <!-- Section 1: Dropdown Selector -->
    <form id="configForm" method="POST" action="/config" target="dummyframe">
        <label for="configSelect">Select Config:</label>
        <select id="configSelect" name="Config">"` +
                    configsToHTML()
                    +
                `</select>
        <button type="submit">Submit Config</button>
    </form>

    <h1>Manual Input</h1>
    
    <!-- Section 2: Numerical Inputs -->
    <form id="inputForm" method="POST" action="/data" target="dummyframe">
        <label for="inputX">X:</label>
        <input type="number" id="inputX" name="X" value="0"><br>

        <label for="inputY">Y:</label>
        <input type="number" id="inputY" name="Y" value="3"><br>

        <label for="inputZ">Z:</label>
        <input type="number" id="inputZ" name="Z" value="10"><br>

        <button type="submit">Submit Inputs</button>
    </form>
</body>
</html>`)
})

app.post('/config', urlencodedParser, (req, res) => {
    // console.log(req.body.Config)
    io.sockets.emit('config', {Config: req.body.Config});
    res.send({
        message: 'Config Selection Sent!',
    });
});

app.post('/data', urlencodedParser, (req, res) => {
    // console.log(`${req.body.X},${req.body.Y},${req.body.Z}`)
    io.sockets.emit('data', {X: req.body.X, Y: req.body.Y, Z: req.body.Z});

    res.send({
        message: 'Config Selection Sent!',
    });
});

app.listen(port, () => {
    console.log(`Listening On Port ${port}`)
})










function getConfigs() {
    var PATH = `C:\\Users\\${process.env.USERNAME}\\AppData\\Local\\FAR\\Configs`;
    return fs.readdirSync(PATH);
}
function configsToHTML() {
    var fullCode = "";
    var configs = getConfigs();
    configs.forEach(config => {
        var configName = config.replace(".json", "");
        var code = `<option value="${configName}">${configName}</option>`;
        fullCode = fullCode + code;
    });
    return fullCode;
}