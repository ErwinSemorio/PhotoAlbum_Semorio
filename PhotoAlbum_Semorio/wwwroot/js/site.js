// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// site.js - lightbox + scroll reveal + page loader
document.addEventListener('DOMContentLoaded', function () {
  // --- PAGE LOADER ---
  const loader = document.createElement('div');
  loader.id = 'page-loader';
  loader.innerHTML = '<div class="spinner" role="status" aria-hidden="true"></div>';
  document.body.appendChild(loader);

  // hide loader after window load (keeps loader if async resources still loading)
  window.addEventListener('load', function () {
    loader.classList.add('hidden');
    setTimeout(() => loader.remove(), 600);
  });

  // --- LIGHTBOX (existing minimal implementation) ---
  const lb = document.createElement('div');
  lb.className = 'lightbox';
  lb.innerHTML = '<button class="close" aria-label="Close">&times;</button><img alt=""><div class="caption"></div>';
  document.body.appendChild(lb);

  const lbImg = lb.querySelector('img');
  const caption = lb.querySelector('.caption');
  const closeBtn = lb.querySelector('.close');

  document.body.addEventListener('click', function (e) {
    const a = e.target.closest('a.photo-link');
    if (!a) return;
    e.preventDefault();
    const src = a.getAttribute('href') || a.querySelector('img')?.src;
    const cap = a.dataset.caption || a.title || '';
    lbImg.src = src;
    caption.textContent = cap;
    lb.classList.add('active');
    document.body.style.overflow = 'hidden';
  });

  function closeLightbox() {
    lb.classList.remove('active');
    lbImg.src = '';
    caption.textContent = '';
    document.body.style.overflow = '';
  }

  closeBtn.addEventListener('click', closeLightbox);
  lb.addEventListener('click', function (e) {
    if (e.target === lb || e.target === closeBtn) closeLightbox();
  });
  document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') closeLightbox();
  });

  // --- SCROLL REVEAL (IntersectionObserver) ---
  const revealTargets = document.querySelectorAll('.photo-card, .gallery-title, .main-title, .page-links a');
  if ('IntersectionObserver' in window) {
    const io = new IntersectionObserver((entries, obs) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          // small stagger based on index for smoother feel
          const el = entry.target;
          const parent = el.parentElement;
          let idx = 0;
          if (parent) {
            idx = Array.prototype.indexOf.call(parent.children, el) % 10;
          }
          el.style.transitionDelay = (idx * 40) + 'ms';
          el.classList.add('revealed');
          obs.unobserve(el);
        }
      });
    }, { threshold: 0.12 });

    revealTargets.forEach(t => io.observe(t));
  } else {
    // fallback: reveal all
    revealTargets.forEach(t => t.classList.add('revealed'));
  }

  // Optional small accessibility improvement: keyboard open on image links
  document.body.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
      const focused = document.activeElement;
      if (focused && focused.matches && focused.matches('a.photo-link')) {
        focused.click();
      }
    }
  });
});
