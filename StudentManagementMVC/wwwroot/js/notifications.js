// SignalR Notification Client
(function () {
    'use strict';

    // Khởi tạo SignalR connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect()
        .build();

    // Counter cho số thông báo chưa đọc
    let unreadCount = 0;

    // Kết nối
    connection.start()
        .then(() => {
            console.log("✅ SignalR Connected: Notification Hub");
            loadUnreadNotifications();
        })
        .catch(err => console.error("❌ SignalR Connection Error:", err));

    // Xử lý reconnect
    connection.onreconnected(() => {
        console.log("🔄 SignalR Reconnected");
        loadUnreadNotifications();
    });

    // Nhận notification mới từ server
    connection.on("ReceiveNotification", (notification) => {
        console.log("📬 New Notification:", notification);
        
        // Tăng counter
        unreadCount++;
        updateNotificationBadge();

        // Hiển thị toast
        showNotificationToast(notification);

        // Thêm vào dropdown list
        addNotificationToList(notification);
    });

    // Load số lượng notification chưa đọc khi page load
    function loadUnreadNotifications() {
        fetch('/Notifications/GetUnreadCount')
            .then(response => response.json())
            .then(data => {
                unreadCount = data.count || 0;
                updateNotificationBadge();
            })
            .catch(err => console.error("Error loading unread count:", err));
    }

    // Update badge hiển thị số notification
    function updateNotificationBadge() {
        const badge = document.querySelector('.notification-badge');
        if (badge) {
            if (unreadCount > 0) {
                badge.textContent = unreadCount > 99 ? '99+' : unreadCount;
                badge.style.display = 'inline-block';
            } else {
                badge.style.display = 'none';
            }
        }
    }

    // Hiển thị toast notification
    function showNotificationToast(notification) {
        // Sử dụng Toast library hoặc tự tạo
        const toastHtml = `
            <div class="toast-notification" style="
                position: fixed; 
                top: 80px; 
                right: 20px; 
                background: white; 
                padding: 15px 20px; 
                border-radius: 8px; 
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                border-left: 4px solid ${getNotificationColor(notification.type)};
                max-width: 350px;
                animation: slideIn 0.3s ease-out;
                z-index: 9999;">
                <div style="display: flex; align-items: center; gap: 10px;">
                    <i class="fas ${getNotificationIcon(notification.type)}" 
                       style="color: ${getNotificationColor(notification.type)}; font-size: 20px;"></i>
                    <div style="flex: 1;">
                        <strong style="display: block; margin-bottom: 5px;">${notification.title}</strong>
                        <small style="color: #666;">${notification.message}</small>
                    </div>
                    <button onclick="this.parentElement.parentElement.remove()" 
                            style="border: none; background: none; cursor: pointer; color: #999;">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            </div>
        `;

        const div = document.createElement('div');
        div.innerHTML = toastHtml;
        document.body.appendChild(div.firstElementChild);

        // Auto remove sau 5 giây
        setTimeout(() => {
            const toast = document.querySelector('.toast-notification');
            if (toast) {
                toast.style.animation = 'slideOut 0.3s ease-in';
                setTimeout(() => toast.remove(), 300);
            }
        }, 5000);
    }

    // Thêm notification vào dropdown list
    function addNotificationToList(notification) {
        const list = document.querySelector('.notification-dropdown-list');
        if (!list) return;

        const item = document.createElement('a');
        item.className = 'dropdown-item notification-item unread';
        item.href = notification.link || '#';
        item.innerHTML = `
            <div class="d-flex align-items-center">
                <div class="notification-icon ${notification.type}">
                    <i class="fas ${getNotificationIcon(notification.type)}"></i>
                </div>
                <div class="flex-grow-1 ms-3">
                    <strong>${notification.title}</strong>
                    <p class="mb-0 text-muted small">${notification.message}</p>
                    <small class="text-muted">${getRelativeTime(notification.createdAt)}</small>
                </div>
            </div>
        `;

        // Thêm vào đầu list
        list.insertBefore(item, list.firstChild);
    }

    // Helper functions
    function getNotificationIcon(type) {
        const icons = {
            'success': 'fa-check-circle',
            'info': 'fa-info-circle',
            'warning': 'fa-exclamation-triangle',
            'error': 'fa-times-circle',
            'score': 'fa-star',
            'ai': 'fa-brain',
            'default': 'fa-bell'
        };
        return icons[type] || icons.default;
    }

    function getNotificationColor(type) {
        const colors = {
            'success': '#4caf50',
            'info': '#2196f3',
            'warning': '#ff9800',
            'error': '#f44336',
            'score': '#ffc107',
            'ai': '#9c27b0',
            'default': '#607d8b'
        };
        return colors[type] || colors.default;
    }

    function getRelativeTime(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffMs = now - date;
        const diffMins = Math.floor(diffMs / 60000);
        
        if (diffMins < 1) return 'Vừa xong';
        if (diffMins < 60) return `${diffMins} phút trước`;
        if (diffMins < 1440) return `${Math.floor(diffMins / 60)} giờ trước`;
        return `${Math.floor(diffMins / 1440)} ngày trước`;
    }

    // CSS Animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from {
                transform: translateX(400px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(400px);
                opacity: 0;
            }
        }
        .notification-badge {
            position: absolute;
            top: -5px;
            right: -5px;
            background: #f44336;
            color: white;
            border-radius: 50%;
            width: 20px;
            height: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 11px;
            font-weight: bold;
        }
    `;
    document.head.appendChild(style);

    // Export connection để sử dụng ở nơi khác
    window.notificationHub = connection;
})();
