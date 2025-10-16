const links = document.querySelectorAll('.link');

links.forEach(link => {
	if (link.getAttribute('href') === window.location.pathname) {
		link.classList.add('active-state');
	}
});

document.addEventListener("DOMContentLoaded", function () {
	const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
	const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
});