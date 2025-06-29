document.addEventListener('DOMContentLoaded', () => {
    const phone = document.querySelector('#phone');
    const theForm = phone?.closest('form');

    if (!phone || !theForm) return;

    const iti = window.intlTelInput(phone, {
        initialCountry: 'pl',   
        separateDialCode: true,      
        nationalMode: false,    
        utilsScript: 'https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/18.4.1/js/utils.js'
    });

    phone.addEventListener('input', () => {
        phone.value = phone.value.replace(/\D/g, '');

        const maxLen = iti.getNumberType() === intlTelInputUtils.numberType.MOBILE
            ? 15 : 15;             
        if (phone.value.length > maxLen) {
            phone.value = phone.value.slice(0, maxLen);
        }
    });

    theForm.addEventListener('submit', () => {
        phone.value = iti.getNumber();       
    });
});