(function () {
    const colors = ["#f7d154", "#ff6b8a", "#5f8cff", "#39d98a", "#ffffff", "#9ee7ff"];

    function burst(originX, originY) {
        const canvas = document.createElement("canvas");
        const context = canvas.getContext("2d");
        const ratio = window.devicePixelRatio || 1;
        const pieces = Array.from({ length: 90 }, () => {
            const angle = Math.random() * Math.PI * 2;
            const speed = 4 + Math.random() * 7;
            return {
                x: originX * window.innerWidth,
                y: originY * window.innerHeight,
                vx: Math.cos(angle) * speed,
                vy: Math.sin(angle) * speed - 3,
                rotation: Math.random() * Math.PI,
                spin: (Math.random() - 0.5) * 0.35,
                size: 5 + Math.random() * 7,
                color: colors[Math.floor(Math.random() * colors.length)],
                life: 80 + Math.random() * 35
            };
        });

        canvas.style.position = "fixed";
        canvas.style.inset = "0";
        canvas.style.pointerEvents = "none";
        canvas.style.zIndex = "1000";
        canvas.width = window.innerWidth * ratio;
        canvas.height = window.innerHeight * ratio;
        context.scale(ratio, ratio);
        document.body.appendChild(canvas);

        let frame = 0;
        function draw() {
            frame += 1;
            context.clearRect(0, 0, window.innerWidth, window.innerHeight);

            for (const piece of pieces) {
                piece.x += piece.vx;
                piece.y += piece.vy;
                piece.vy += 0.11;
                piece.vx *= 0.985;
                piece.rotation += piece.spin;

                const alpha = Math.max(0, 1 - frame / piece.life);
                context.save();
                context.globalAlpha = alpha;
                context.translate(piece.x, piece.y);
                context.rotate(piece.rotation);
                context.fillStyle = piece.color;
                context.fillRect(-piece.size / 2, -piece.size / 2, piece.size, piece.size * 0.58);
                context.restore();
            }

            if (frame < 115) {
                requestAnimationFrame(draw);
            } else {
                canvas.remove();
            }
        }

        requestAnimationFrame(draw);
    }

    window.pilkkiConfetti = {
        fireworks() {
            burst(0.18, 0.32);
            setTimeout(() => burst(0.82, 0.28), 230);
            setTimeout(() => burst(0.5, 0.22), 470);
        }
    };
})();
