
window.dashboard = (function(){
  function renderDonut(labels, values){
    const ctx = document.getElementById('statusDonut');
    if (!ctx) return;
    const data = {
      labels: Array.from(labels),
      datasets: [{
        data: Array.from(values),
        // Colors must match badges palette
        backgroundColor: ['#0ea5e9','#fbbf24','#3b82f6','#6366f1','#22c55e','#ef4444'],
        borderWidth: 0
      }]
    };
    if (!window.Chart){
      console.warn('Chart.js not loaded');
      return;
    }
    new Chart(ctx, {
      type: 'doughnut',
      data: data,
      options: {
        plugins: { legend: { labels: { color: getTextColor() } } },
        cutout: '60%'
      }
    });
  }

  function getTextColor(){
    const style = getComputedStyle(document.documentElement);
    return style.getPropertyValue('--text') || '#222';
  }

  return { renderDonut };
})();

function groupDaily(daily){
  const map = new Map();
  const labelsSet = new Set();
  for (const p of daily){
    const d = p.date.toString();
    labelsSet.add(d);
    const key = p.status;
    if (!map.has(key)) map.set(key, new Map());
    map.get(key).set(d, p.count);
  }
  const labels = Array.from(labelsSet).sort();
  for (const m of map.values()){
    for (const L of labels){
      if (!m.has(L)) m.set(L, 0);
    }
  }
  return { labels, series: map };
}

function seriesColor(status){
  switch(status){
    case 'Received': return '#0ea5e9';
    case 'Printing': return '#fbbf24';
    case 'Inserting': return '#3b82f6';
    case 'Mailed': return '#6366f1';
    case 'Delivered': return '#22c55e';
    case 'Exception': return '#ef4444';
    default: return '#94a3b8';
  }
}

function renderLine(daily){
  const el = document.getElementById('statusLine');
  if (!el || !window.Chart) return;
  const { labels, series } = groupDaily(daily);
  const datasets = [];
  for (const [status, points] of series.entries()){
    const prominent = status === 'Delivered' || status === 'Exception';
    datasets.push({
      label: status,
      data: labels.map(d => points.get(d) || 0),
      borderColor: seriesColor(status),
      backgroundColor: seriesColor(status),
      tension: 0.3,
      borderWidth: prominent ? 2 : 1,
      fill: false,
      hidden: !prominent
    });
  }
  new Chart(el, {
    type: 'line',
    data: { labels, datasets },
    options: {
      responsive: true,
      plugins: { legend: { labels: { color: getTextColor() } } },
      scales: {
        x: { ticks: { color: getTextColor() }, grid: { color: '#27304222' } },
        y: { ticks: { color: getTextColor() }, grid: { color: '#27304222' }, beginAtZero: true }
      }
    }
  });
}

window.dashboard.renderLine = renderLine;
