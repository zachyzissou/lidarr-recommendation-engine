(() => {
  const list = () => document.getElementById('list');
  const sortSel = () => document.getElementById('sort');
  let currentTab = 'similar';

  function render(items) {
    const el = list();
    el.innerHTML = '';
    for (const it of items) {
      const div = document.createElement('div');
      div.className = 'item';
      div.innerHTML = `
        <div class="title">${it.title}</div>
        <div class="subtitle">${it.subtitle ?? ''}</div>
        <div class="score">Score: ${it.score.toFixed(2)}</div>
        <div class="reason">${it.reason?.summary ?? ''}</div>
        <div class="actions">
          <button>Add</button>
          <button>Wanted</button>
          <button>Hide</button>
        </div>`;
      el.appendChild(div);
    }
  }

  async function fetchRecs(kind) {
    // In real SDK, call plugin API; here use placeholder
    return [];
  }

  async function refresh() {
    const items = await fetchRecs(currentTab);
    render(items);
  }

  document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.tabs button').forEach(btn => {
      btn.addEventListener('click', () => {
        document.querySelectorAll('.tabs button').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        currentTab = btn.dataset.tab;
        refresh();
      });
    });
    sortSel().addEventListener('change', refresh);
    refresh();
  });
})();
(function(){
  const list = document.getElementById('list');
  const tab = document.getElementById('tab');
  const sort = document.getElementById('sort');

  async function fetchRecs(kind){
    // In real SDK, call plugin API endpoint; here we stub
    console.log('Fetch recs (stub):', kind);
    return [];
  }

  function render(items){
    list.innerHTML = '';
    for(const r of items){
      const li = document.createElement('li');
      li.className = 'item';
      li.innerHTML = `<div><strong>${r.title}</strong>${r.subtitle? ` <span>— ${r.subtitle}</span>`:''}<div class="reason">${r.reason?.summary||''}</div></div>` +
        `<div class="actions">`+
        `<button>Add</button>`+
        `<button class="secondary">Hide</button>`+
        `</div>`;
      list.appendChild(li);
    }
  }

  async function load(){
    const items = await fetchRecs(tab.value);
    render(items);
  }
  tab.addEventListener('change', load);
  sort.addEventListener('change', load);
  load();
})();
