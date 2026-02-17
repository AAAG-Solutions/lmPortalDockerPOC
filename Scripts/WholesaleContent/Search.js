let stream = null;
let animationId = null;
let isScanning = false;
let lastDetectedBarcodes = [];
let barcodeTimestamps = new Map();
const DECAY_TIME = 2000; // Keep barcodes visible for 2 seconds

var video = null;
var canvas = null;
var ctx = null;
var overlay = null;
var startBtn = null;
var stopBtn = null;
var placeholder = null;
var errorDiv = null;
var resultsDiv = null;
var barcodeList = null;

async function startScanning() {
    try {
        hideError();
        stream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: 'environment' }
        });

        video.srcObject = stream;
        video.onloadedmetadata = () => {
            isScanning = true;
            placeholder.classList.add('hidden');
            video.classList.remove('hidden');
            canvas.classList.remove('hidden');
            overlay.classList.remove('hidden');
            startBtn.classList.add('hidden');
            stopBtn.classList.remove('hidden');

            // Set overlay viewBox to match video dimensions
            overlay.setAttribute('viewBox', `0 0 ${video.videoWidth} ${video.videoHeight}`);

            detectBarcodes();
        };
    } catch (err) {
        showError('Camera access denied or not available: ' + err.message);
    }
}

function stopScanning() {
    if (stream) {
        stream.getTracks().forEach(track => track.stop());
        stream = null;
    }
    if (animationId) {
        cancelAnimationFrame(animationId);
        animationId = null;
    }

    isScanning = false;
    barcodeTimestamps.clear();
    video.classList.add('hidden');
    canvas.classList.add('hidden');
    overlay.classList.add('hidden');
    overlay.innerHTML = '';
    placeholder.classList.remove('hidden');
    startBtn.classList.remove('hidden');
    stopBtn.classList.add('hidden');
    resultsDiv.classList.add('hidden');

    ctx.clearRect(0, 0, canvas.width, canvas.height);
}

async function detectBarcodes() {
    if (!isScanning || video.readyState !== video.HAVE_ENOUGH_DATA) {
        animationId = requestAnimationFrame(detectBarcodes);
        return;
    }

    // Set canvas size to match video
    if (canvas.width !== video.videoWidth || canvas.height !== video.videoHeight) {
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        overlay.setAttribute('viewBox', `0 0 ${video.videoWidth} ${video.videoHeight}`);
    }

    try {
        const barcodeDetector = new BarcodeDetector();
        const detectedBarcodes = await barcodeDetector.detect(video);

        const currentTime = Date.now();

        // Update timestamps for detected barcodes
        detectedBarcodes.forEach(barcode => {
            if (barcode.rawValue.length == 17) {
                var resultContainer = document.getElementById('MainContent_txtSearch');
                resultContainer.value = barcode.rawValue;
                ToggleScanner();
                document.getElementById("VinSearchButton").click();
            }

            if (barcode.rawValue.length == 18 && barcode.rawValue[0] == "i") {
                var result = barcode.rawValue.substr(1);
                var resultContainer = document.getElementById('MainContent_txtSearch');
                resultContainer.value = result;
                ToggleScanner();
                document.getElementById("VinSearchButton").click();
            }

            const key = `${barcode.format}:${barcode.rawValue}`;
            barcodeTimestamps.set(key, {
                barcode: barcode,
                time: currentTime
            });
        });

        // Remove expired barcodes
        for (let [key, data] of barcodeTimestamps.entries()) {
            if (currentTime - data.time > DECAY_TIME) {
                barcodeTimestamps.delete(key);
            }
        }

        // Clear and redraw SVG overlay
        overlay.innerHTML = '';

        // Draw all active barcodes (including decaying ones)
        const activeBarcodes = [];
        for (let [key, data] of barcodeTimestamps.entries()) {
            const barcode = data.barcode;
            const age = currentTime - data.time;
            const opacity = Math.max(0, 1 - (age / DECAY_TIME));

            activeBarcodes.push(barcode);

            // Draw polygon from corner points
            if (barcode.cornerPoints && barcode.cornerPoints.length > 0) {
                const points = barcode.cornerPoints.map(p => `${p.x},${p.y}`).join(' ');

                const polygon = document.createElementNS('http://www.w3.org/2000/svg', 'polygon');
                polygon.setAttribute('points', points);
                polygon.setAttribute('class', 'barcode-polygon');
                polygon.style.opacity = opacity;
                overlay.appendChild(polygon);

                // Add text label
                const firstPoint = barcode.cornerPoints[0];
                const textY = firstPoint.y - 10;

                // Background for text
                const textBg = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
                const textWidth = barcode.rawValue.length * 10 + 10;
                textBg.setAttribute('x', firstPoint.x);
                textBg.setAttribute('y', textY - 20);
                textBg.setAttribute('width', textWidth);
                textBg.setAttribute('height', 25);
                textBg.setAttribute('class', 'barcode-text-bg');
                textBg.style.opacity = opacity;
                overlay.appendChild(textBg);

                // Text
                const text = document.createElementNS('http://www.w3.org/2000/svg', 'text');
                text.setAttribute('x', firstPoint.x + 5);
                text.setAttribute('y', textY);
                text.setAttribute('class', 'barcode-text');
                text.textContent = barcode.rawValue;
                text.style.opacity = opacity;
                overlay.appendChild(text);
            }
        }

        if (activeBarcodes.length > 0) {
            displayBarcodes(activeBarcodes);
        } else {
            resultsDiv.classList.add('hidden');
        }
    } catch (err) {
        console.error('Barcode detection error:', err);
    }

    animationId = requestAnimationFrame(detectBarcodes);
}

function displayBarcodes(barcodes) {
    barcodeList.innerHTML = '';

    barcodes.forEach((barcode, index) => {
        const item = document.createElement('div');
        item.className = 'barcode-item';
        item.innerHTML = `
                    <div class="barcode-row">
                        <div class="barcode-col">
                            <div class="barcode-label">Format</div>
                            <div class="barcode-value">${barcode.format}</div>
                        </div>
                        <div class="barcode-col">
                            <div class="barcode-label">Value</div>
                            <div class="barcode-value">${barcode.rawValue}</div>
                        </div>
                    </div>
                `;
        barcodeList.appendChild(item);
    });

    resultsDiv.classList.remove('hidden');
}

function showError(message) {
    errorDiv.textContent = message;
    errorDiv.classList.remove('hidden');
}

function hideError() {
    errorDiv.classList.add('hidden');
}

// Cleanup on page unload
window.addEventListener('beforeunload', () => {
    if (stream) {
        stream.getTracks().forEach(track => track.stop());
    }
});

function SubmitVinSearch() {
    var gridData = $("#jsGrid").data("JSGrid");
    var gfilter = gridData.getFilter();

    // Check VIN if invalid characters are attempted to be searched
    // NOTE: '-' is added in the mix due to VIN creation from DX1 Import
    var vin = document.getElementById('MainContent_txtSearch').value;
    var invalid = document.getElementById('invalidVin');
    var search = document.getElementById('invalidVinLength');

    search.style.display = invalid.style.display = "none";
    if (IsLiquidConnect() && vin == "") {
        toggleLoading(true, "Navigating to Dealer...");
        window.location.href = "/WholesaleContent/VehicleManagement.aspx"
    }
    if (/[^A-Z a-z0-9\-]/.test(vin) || vin == "") {
        document.getElementById('invalidVin').style.display = "block";
        return false;
    } else if (vin.length < 6) {
        document.getElementById('invalidVinLength').style.display = "block";
        return false;
    }

    gfilter.VIN = vin;
    gridData.search(gfilter);
}

function GoToAdd() {
    if (IsLiquidConnect()) {
        toggleLoading(true, "Navigating to add vehicle...");
        window.location.href = "/WholesaleContent/Vehicle/Add.aspx?VIN=" + document.getElementById('MainContent_txtSearch').value;
    }
}

function LCGridProcess() {
    if (IsLiquidConnect()) {
        var gridData = $("#jsGrid").data("JSGrid");
        if (gridData.data.length == 1)
            AssignkDealer(gridData.data[0].kDealer, gridData.data[0].VIN);
        else if (gridData.data.length == 0 && document.getElementById('MainContent_txtSearch').value != "")
            NoVehicleFound()
    }
}

function NoVehicleFound() {
    toggleCssClass([['InfoPopup', 'show_display']])
    document.getElementById("InfoText").innerHTML = "No vehicle was found with that VIN. Do you want to add it to the account"
}

function AssignkDealer(kDealer, vin) {
    $('MainContent.DealerText').val(kDealer);
    $('MainContent.VehicleVin').val(vin);
    var submitButton = $('MainContent.AccountButton');
    toggleLoading(true, "Navigating to Dealer...");
    submitButton.click();
}

function AssignkListing(kDealer, kListing) {
    $('MainContent.VehiclekListing').val(kListing);
    $('MainContent.DealerText').val(kDealer);
    var submitButton = $('MainContent.VehicleButton');
    toggleLoading(true, "Navigating to Vehicle...");
    submitButton.click();
}

$(document).keypress(function (e) {
    if (e.which == 13) {
        var search = document.getElementById("VinSearchButton");
        search.click();
        return false;
    }
});

document.addEventListener("keydown", function (e) {
    if (e.key == "Escape" && document.getElementById("scannerPopup").classList.contains("show_display")) {
        toggleCssClass([['scannerPopup', 'show_display']])
    }
});

function resizeImage(file, maxWidth, callback) {
    const img = new Image();
    const reader = new FileReader();
    reader.onload = function (e) {
        img.onload = function () {
            const canvas = document.createElement('canvas');
            const scale = maxWidth / img.width;
            canvas.width = maxWidth;
            canvas.height = img.height * scale;
            const ctx = canvas.getContext('2d');
            ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
            callback(canvas.toDataURL('image/png'));
        }
        img.src = e.target.result;
    }
    reader.readAsDataURL(file);
}

$(document).ready(function () {
    if (IsLiquidConnect()) {
        document.getElementById("jsGrid").style.display = "none";
        document.getElementById("DesktopActionsHolder").style.display = "none";
        document.getElementById("vehicleActionsBar").style.display = "flex"
        document.getElementById("vehicleActionsBar").style.justifyContent = "center"

        video = document.getElementById('video');
        canvas = document.getElementById('canvas');
        ctx = canvas.getContext('2d');
        overlay = document.getElementById('overlay');
        startBtn = document.getElementById('startBtn');
        stopBtn = document.getElementById('stopBtn');
        placeholder = document.getElementById('placeholder');
        errorDiv = document.getElementById('error');
        resultsDiv = document.getElementById('results');
        barcodeList = document.getElementById('barcodeList');

        // Check if Barcode Detection API is supported
        if (!('BarcodeDetector' in window)) {
            showError('Barcode Detection API is not supported in this browser. Please use Chrome, Edge, or another Chromium-based browser.');
            startBtn.disabled = true;
        }

        startBtn.addEventListener('click', startScanning);
        stopBtn.addEventListener('click', stopScanning);
    }
    else {
        document.getElementById("Scanner").style.display = "none"
        document.getElementById("ScannerButton").style.display = "none"
        document.getElementById("OrText").style.display = "none"
    }
});

function ToggleScanner() {
    toggleCssClass([['scannerPopup', 'show_display']]);
}

gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });