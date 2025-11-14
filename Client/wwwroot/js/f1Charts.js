window.f1Charts = {
    initStintChart: function (canvasId, lapLabels, data) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        if (ctx._chart) ctx._chart.destroy();


        const compoundColors = {
            Soft: '#DA291C',      
            Medium: '#FFD12E',    
            Hard: '#FFFFFF',     
            Intermediate: '#43B02A', 
            Wet: '#0067AD'        
        };

        const drivers = data.map(driver => driver.full_name);
        // Build chronological stint list per driver
        const stintData = data.map(driver => {
            const stints = [];
            const compoundEndLap = driver.race?.compound_end_lap || {};
            for (const [compound, laps] of Object.entries(compoundEndLap)) {
                laps.forEach(endLap => {stints.push({ compound, endLap });});
            }
            // Sort each driver’s stints chronologically by end lap
            stints.sort((a, b) => a.endLap - b.endLap);
            return stints;
        });

        // Find the maximum number of stints across all drivers
        const maxStints = Math.max(...stintData.map(s => s.length));

        // Build one dataset per stint index (so 1st stint bottom, 2nd above, etc.)
        const datasets = [];
        for (let stintIndex = 0; stintIndex < maxStints; stintIndex++) {
            datasets.push({
                label: '',
                data: stintData.map(driverStints => {
                    const stint = driverStints[stintIndex];
                    if (!stint) return 0;
                    const prevEnd = driverStints[stintIndex - 1]?.endLap || 0;
                    return stint.endLap - prevEnd; // stint length
                }),
                backgroundColor: stintData.map(driverStints => {
                    const stint = driverStints[stintIndex];
                    return stint ? compoundColors[stint.compound] : "transparent";
                }),
                borderColor: "#000",
                borderWidth: 1,
            });
        }

        ctx._chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: drivers,
                datasets: datasets
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false, position: 'top' },
                    title: { display: false, text: 'Compound' },
                    tooltip: {
                        callbacks: {
                            label: ctx => `${ctx.dataset.label}: ${ctx.raw} laps`
                        }
                    }
                },
                scales: {
                    x: {
                        stacked: true,
                        title: { display: true, text: 'Drivers' }
                    },
                    y: {
                        stacked: true,
                        beginAtZero: true,
                        title: { display: true, text: 'Laps' }
                    }
                }
            }
        });
    },

    initLapChart: function (canvasId, labels, datasets) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        if (ctx._chart) ctx._chart.destroy();

        const colors = [
            '#E10600', '#1E41FF', '#00D2BE', '#FF8700', '#2D826D',
            '#9B9B9B', '#FFD700', '#7ED321', '#D0021B', '#00A7E1',
            '#FF66CC', '#A020F0', '#FF4500', '#008000', '#00CED1',
            '#C71585', '#4682B4', '#FFDAB9', '#8B4513', '#F0E68C'
        ];

        // If there are more drivers than colors, generate unique hues dynamically
        function getColor(index) {
            if (index < colors.length) return colors[index];
            const hue = (index * 37) % 360; // spread hues around the color wheel
            return `hsl(${hue}, 80%, 50%)`;
        }

        const ds = datasets.map((d, i) => ({
            label: d.label,
            data: d.data,
            tension: 0.2,
            borderWidth: 2,
            borderColor: getColor(i),
            fill: false,
            pointRadius: 0,
            borderJoinStyle: 'round',
            borderCapStyle: 'round'
        }));

        ctx._chart = new Chart(ctx, {
            type: 'line',
            data: { labels: labels, datasets: ds },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: 'top', labels: { color: '#ddd' } },
                    tooltip: { mode: 'index', intersect: false }
                },
                interaction: { mode: 'nearest', intersect: false },
                scales: {
                    y: {
                        title: { display: true, text: 'Seconds', color: '#aaa' },
                        ticks: { color: '#aaa' }
                    },
                    x: {
                        title: { display: true, text: 'Lap', color: '#aaa' },
                        ticks: { color: '#aaa' }
                    }
                }
            }
        });
    },

    initMapChart: function(geojsonText) {
        const geojson = JSON.parse(geojsonText);

        // Create or reuse the map element
        if (window.f1MapInstance) {
            window.f1MapInstance.remove();
        }

        const map = L.map('circuit-map', {
            center: [44.343, 11.716], // fallback center (Imola-ish)
            zoom: 14
        });

        window.f1MapInstance = map;

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://openstreetmap.org">OpenStreetMap</a> contributors'
        }).addTo(map);

        const circuitLayer = L.geoJSON(geojson, {
            style: {
                color: '#ff9100',
                weight: 5,
                opacity: 0.9
            }
        }).addTo(map);

        map.fitBounds(circuitLayer.getBounds());
    }
};
