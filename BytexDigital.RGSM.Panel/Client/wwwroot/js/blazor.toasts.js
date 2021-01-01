export function showToast(type, title, body) {
    if (type == 0) {
        iziToast.success({
            title: title,
            message: body,
            position: 'topLeft',
            animateInside: false,
            pauseOnHover: true
        });
    } else if (type == 1) {
        iziToast.warning({
            title: title,
            message: body,
            position: 'topLeft',
            animateInside: false,
            pauseOnHover: true,
        });
    } else if (type == 2) {
        iziToast.error({
            title: title,
            message: body,
            position: 'topLeft',
            animateInside: false,
            pauseOnHover: true,
        });
    } else if (type == 3) {
        iziToast.info({
            title: title,
            message: body,
            position: 'topLeft',
            animateInside: false,
            pauseOnHover: true,
        });
    }
}