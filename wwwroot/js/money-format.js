// money-format.js — auto-format tiền cho mọi <input data-money>
(function () {
    const nf = new Intl.NumberFormat('vi-VN'); // đổi 'en-US' nếu muốn 1,234,567

    function onlyNum(s) { return (s || '').replace(/[^\d.,]/g, ''); }
    function parseMoney(str) {
        let s = onlyNum(str).trim();
        if (!s) return null;
        const lastDot = s.lastIndexOf('.'), lastComma = s.lastIndexOf(',');
        if (lastDot >= 0 && lastComma >= 0) {
            const p = Math.max(lastDot, lastComma);
            s = s.slice(0, p).replace(/[.,]/g, '') + '.' + s.slice(p + 1).replace(/[^\d]/g, '');
        } else if (s.includes(',')) {
            const a = s.split(',');
            s = (a.length === 2 && a[1].length <= 2) ? a[0].replace(/[.,]/g, '') + '.' + a[1] : s.replace(/,/g, '');
        } else if (s.includes('.')) {
            const a = s.split('.');
            s = (a.length === 2 && a[1].length <= 2) ? a[0].replace(/[.,]/g, '') + '.' + a[1] : s.replace(/\./g, '');
        }
        const n = Number(s);
        return Number.isFinite(n) ? n : null;
    }
    function fmt(n) { return (n == null || n === '') ? '' : nf.format(Number(n)); }

    function enhance(input) {
        if (input._moneyEnhanced) return;
        input._moneyEnhanced = true;

        // Nếu input có name → tạo 1 hidden cùng name để post số “sạch”
        const name = input.getAttribute('name');
        let hidden = null;
        if (name) {
            hidden = document.createElement('input');
            hidden.type = 'hidden';
            hidden.name = name;
            hidden.id = input.id ? input.id + '_hidden' : '';
            input.removeAttribute('name'); // tránh post 2 field trùng tên
            input.parentNode.insertBefore(hidden, input.nextSibling);
        }

        const init = hidden ? hidden.value : input.value;
        const num = parseMoney(init);
        if (hidden) hidden.value = (num ?? '').toString();
        input.value = fmt(num);

        // --- thêm vào ---
        function caretToEnd(el) {
            const len = el.value.length;
            try { el.setSelectionRange(len, len); } catch { }
        }

        // Khi focus, đưa caret về cuối để người dùng gõ tiếp tự nhiên
        input.addEventListener('focus', () => {
            // trick nhỏ để đảm bảo trên 1 số trình duyệt
            const v = input.value;
            requestAnimationFrame(() => {
                input.value = v;
                caretToEnd(input);
            });
        });

        // Nếu input đang được focus ngay khi init và có sẵn giá trị -> cũng đưa caret về cuối
        if (document.activeElement === input) {
            caretToEnd(input);
        }

        input.addEventListener('input', () => {
            const n = parseMoney(input.value);
            if (hidden) hidden.value = (n ?? '').toString();
            input.value = fmt(n);
            input.setSelectionRange(input.value.length, input.value.length);
        });
        input.addEventListener('blur', () => {
            const n = parseMoney(input.value);
            if (hidden) hidden.value = (n ?? '').toString();
            input.value = fmt(n);
        });
    }

    function init(root) { root.querySelectorAll('input[data-money]').forEach(enhance); }
    document.addEventListener('DOMContentLoaded', () => init(document));
    window.moneyFormatInit = init; // gọi lại khi thêm dòng động
})();
