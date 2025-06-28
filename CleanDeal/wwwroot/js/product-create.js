
(() => {
    const nameInp = document.getElementById('inpName');
    const priceInp = document.getElementById('inpPrice');
    const qtyInp = document.getElementById('inpQty');
    const catSel = document.getElementById('selCat');

    const prevTitle = document.getElementById('prevTitle');
    const prevPrice = document.getElementById('prevPrice');
    const prevBadge = document.getElementById('prevBadge');
    const prevImg = document.getElementById('prevImg');

    const priceOpts = { style: 'currency', currency: 'PLN' };
    const catColors = { Cleaner: '#14b8a6', Client: '#6366f1' };

    function updatePreview() {
        
        prevTitle.textContent = nameInp.value || 'Nowy produkt';

        const val = parseFloat(priceInp.value.replace(',', '.'));
        prevPrice.textContent = isNaN(val)
            ? '0,00 zł'
            : val.toLocaleString('pl-PL', priceOpts);

        const qty = parseInt(qtyInp.value);
        if (isNaN(qty) || qty <= 0) {
            prevBadge.textContent = 'BRAK';
            prevBadge.style.background = '#dc3545';
        } else {
            prevBadge.textContent = 'NOWOŚĆ';
            prevBadge.style.background = '#10b981';
        }

        /* kolor gradientu wg kategorii */
        const col = catColors[catSel.value] || '#a78bfa';
        prevImg.style.setProperty('--bg-color', col);
    }

    [nameInp, priceInp, qtyInp, catSel].forEach(el =>
        el.addEventListener('input', updatePreview));
    updatePreview();

    /* bootstrap-like walidacja */
    const form = document.querySelector('.needs-validation');
    form.addEventListener('submit', e => {
        if (!form.checkValidity()) { e.preventDefault(); e.stopPropagation(); }
        form.classList.add('was-validated');
    }, false);
})();
