/* danebalia.com — theme toggle, reading progress, active TOC.
   Pair with the no-flash snippet in <head> (see templates). */
(function () {
  var root = document.documentElement;

  // ---- theme toggle ----
  function currentTheme() {
    return root.getAttribute('data-theme') ||
      (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
  }
  function setTheme(t) {
    root.setAttribute('data-theme', t);
    try { localStorage.setItem('theme', t); } catch (e) {}
    document.querySelectorAll('[data-theme-toggle]').forEach(function (b) {
      b.textContent = t === 'dark' ? '\u2600  Light' : '\u263E  Dark';
    });
  }
  document.addEventListener('click', function (e) {
    var btn = e.target.closest('[data-theme-toggle]');
    if (!btn) return;
    setTheme(currentTheme() === 'dark' ? 'light' : 'dark');
  });
  // sync initial label
  document.querySelectorAll('[data-theme-toggle]').forEach(function (b) {
    b.textContent = currentTheme() === 'dark' ? '\u2600  Light' : '\u263E  Dark';
  });

  // ---- reading progress bar (articles only) ----
  var bar = document.querySelector('.read-progress');
  var article = document.querySelector('.prose');
  if (bar && article) {
    var onScroll = function () {
      var rect = article.getBoundingClientRect();
      var total = rect.height - window.innerHeight;
      var passed = -rect.top;
      var pct = total > 0 ? Math.min(100, Math.max(0, (passed / total) * 100)) : 0;
      bar.style.width = pct + '%';
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    window.addEventListener('resize', onScroll);
    onScroll();
  }

  // ---- active TOC link on scroll ----
  var links = Array.prototype.slice.call(document.querySelectorAll('.toc-list a[href^="#"]'));
  if (links.length) {
    var targets = links.map(function (a) {
      return document.getElementById(decodeURIComponent(a.getAttribute('href').slice(1)));
    });
    var spy = function () {
      var idx = 0;
      for (var i = 0; i < targets.length; i++) {
        if (targets[i] && targets[i].getBoundingClientRect().top < 120) idx = i;
      }
      links.forEach(function (a, i) { a.classList.toggle('active', i === idx); });
    };
    window.addEventListener('scroll', spy, { passive: true });
    spy();
  }
})();
