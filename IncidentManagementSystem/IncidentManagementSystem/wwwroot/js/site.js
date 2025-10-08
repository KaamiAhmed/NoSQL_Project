const links = document.querySelectorAll('.link');

links.forEach(link => {
	if (link.getAttribute('href') === window.location.pathname) {
		link.classList.add('active-state');
	}
});