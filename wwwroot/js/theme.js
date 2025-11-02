
window.theme = (function(){
  const KEY = 'printopsflow-theme'; // 'light' | 'dark'
  const DARK_ID = 'dark-css';

  function ensureDarkLink(){
    let link = document.getElementById(DARK_ID);
    if (!link) {
      link = document.createElement('link');
      link.id = DARK_ID;
      link.rel = 'stylesheet';
      link.href = '/css/site.dark.css';
      document.head.appendChild(link);
    }
    return link;
  }

  function apply(theme){
    const t = theme === 'dark' ? 'dark' : 'light';
    localStorage.setItem(KEY, t);
    const link = ensureDarkLink();
    if (t === 'dark'){
      link.disabled = false;
      document.documentElement.setAttribute('data-theme','dark');
    } else {
      link.disabled = true;
      document.documentElement.removeAttribute('data-theme');
    }
    return t;
  }

  function toggle(){
    const current = get();
    return apply(current === 'dark' ? 'light' : 'dark');
  }

  function get(){
    return localStorage.getItem(KEY) || 'light';
  }

  // apply on load
  document.addEventListener('DOMContentLoaded', () => apply(get()));
  return { apply, toggle, get };
})();
