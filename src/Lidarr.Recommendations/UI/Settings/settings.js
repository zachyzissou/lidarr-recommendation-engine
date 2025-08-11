(() => {
  const q = (id) => document.getElementById(id);
  const status = q('status');

  async function load() {
    try {
      // In real SDK, fetch plugin settings via provided API. Here we default.
      const defaults = {
        offlineOnly: true,
        enableListenBrainz: false,
        enableMusicBrainz: false,
        novelty: 0.5,
        minPopularity: 0.2,
        includeGenres: [],
        excludeGenres: [],
        hideLiveAndCompilations: true,
        autoTagAccepted: true,
        preferLossless: false,
      };
      q('offlineOnly').checked = defaults.offlineOnly;
      q('enableListenBrainz').checked = defaults.enableListenBrainz;
      q('enableMusicBrainz').checked = defaults.enableMusicBrainz;
      q('novelty').value = defaults.novelty;
      q('minPopularity').value = defaults.minPopularity;
      q('includeGenres').value = defaults.includeGenres.join(',');
      q('excludeGenres').value = defaults.excludeGenres.join(',');
      q('hideLiveAndCompilations').checked = defaults.hideLiveAndCompilations;
      q('autoTagAccepted').checked = defaults.autoTagAccepted;
      q('preferLossless').checked = defaults.preferLossless;
    } catch (e) {
      status.textContent = 'Failed to load settings';
    }
  }

  async function save() {
    try {
      const payload = {
        offlineOnly: q('offlineOnly').checked,
        enableListenBrainz: q('enableListenBrainz').checked,
        enableMusicBrainz: q('enableMusicBrainz').checked,
        novelty: Number(q('novelty').value),
        minPopularity: Number(q('minPopularity').value),
        includeGenres: q('includeGenres').value.split(',').map(s=>s.trim()).filter(Boolean),
        excludeGenres: q('excludeGenres').value.split(',').map(s=>s.trim()).filter(Boolean),
        hideLiveAndCompilations: q('hideLiveAndCompilations').checked,
        autoTagAccepted: q('autoTagAccepted').checked,
        preferLossless: q('preferLossless').checked,
      };
      // In real SDK, POST to plugin settings save endpoint
      console.log('Save settings', payload);
      status.textContent = 'Saved';
      setTimeout(()=> status.textContent='', 1500);
    } catch (e) {
      status.textContent = 'Failed to save settings';
    }
  }

  function recompute() {
    // In real SDK, call clear cache/recompute endpoint
    status.textContent = 'Recompute requested';
    setTimeout(()=> status.textContent='', 1500);
  }

  document.addEventListener('DOMContentLoaded', () => {
    load();
    q('btnSave').addEventListener('click', save);
    q('btnRecompute').addEventListener('click', recompute);
  });
})();
(function(){
  const q = (id)=>document.getElementById(id);
  const settingsKeys = [
    'offlineOnly','enableListenBrainz','enableMusicBrainz','novelty','minPopularity'
  ];
  function load(){
    // Placeholder: In real SDK, fetch settings via host API
    console.log('Settings load: stub');
  }
  function save(){
    const payload = Object.fromEntries(settingsKeys.map(k=>[k, q(k).type==='checkbox'? q(k).checked : parseFloat(q(k).value)]));
    console.log('Settings save: stub', payload);
    alert('Saved (stub)');
  }
  q('recompute').addEventListener('click', ()=>{
    console.log('Recompute clicked (stub)');
    alert('Recompute triggered (stub)');
  });
  document.addEventListener('change', (e)=>{
    if (settingsKeys.includes(e.target.id)) save();
  });
  load();
})();
