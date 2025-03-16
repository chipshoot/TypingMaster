window.initializeSubmenu = function () {
    document.addEventListener('DOMContentLoaded', function () {

        console.log("inside initializeSubmenu");
        // Handle dropdown submenu hover/click
        const dropdownSubmenus = document.querySelectorAll('.dropdown-submenu');
        
        dropdownSubmenus.forEach(function (dropdownSubmenu) {
            const dropdownToggle = dropdownSubmenu.querySelector('.dropdown-toggle');
            
            // For desktop: use hover
            dropdownSubmenu.addEventListener('mouseenter', function () {
                this.querySelector('.dropdown-menu').classList.add('show');
            });
            
            dropdownSubmenu.addEventListener('mouseleave', function () {
                this.querySelector('.dropdown-menu').classList.remove('show');
            });
            
            // For mobile: use click
            dropdownToggle.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                const submenu = this.nextElementSibling;
                
                // Toggle the submenu
                if (submenu.classList.contains('show')) {
                    submenu.classList.remove('show');
                } else {
                    submenu.classList.add('show');
                }
            });
        });
    });
};