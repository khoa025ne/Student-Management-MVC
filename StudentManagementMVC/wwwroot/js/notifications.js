// Notification System - SignalR + REST API Integration
(function () {
    'use strict';

    // Khởi tạo SignalR connection (nếu có hub)
    let connection = null;
    if (typeof signalR !== 'undefined') {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => {
                console.log("✅ SignalR Connected: Notification Hub");
                loadUnreadNotifications();
            })
            .catch(err => console.error("❌ SignalR Connection Error:", err));

        connection.onreconnected(() => {
            console.log("🔄 SignalR Reconnected");
            loadUnreadNotifications();
        });

        // Nhận notification mới từ server
        connection.on("ReceiveNotification", (notification) => {
            console.log("📬 New Notification:", notification);
            unreadCount++;
            updateNotificationBadge();
            showNotificationToast(notification);
            addNotificationToDropdown(notification);
        });
    }

    // Counter cho số thông báo chưa đọc
    let unreadCount = 0;

    // DOM Elements
    const bellButton = document.getElementById('notificationBell');
    const bellButtonMobile = document.getElementById('notificationBellMobile');
    const dropdown = document.getElementById('notificationDropdown');
    const countBadge = document.getElementById('notificationCount');
    const countBadgeMobile = document.getElementById('notificationCountMobile');
    const notificationList = document.getElementById('notificationList');

    // Toggle dropdown
    function toggleDropdown(e) {
        e?.stopPropagation();
        dropdown?.classList.toggle('show');
        
        // Load notifications khi mở dropdown lần đầu
        if (dropdown?.classList.contains('show') && !dropdown.dataset.loaded) {
            loadNotifications();
            dropdown.dataset.loaded = 'true';
        }
    }

    bellButton?.addEventListener('click', toggleDropdown);
    bellButtonMobile?.addEventListener('click', toggleDropdown);

    // Close dropdown when clicking outside
    document.addEventListener('click', function(e) {
        if (dropdown && !dropdown.contains(e.target) && 
            e.target !== bellButton && e.target !== bellButtonMobile) {
            dropdown.classList.remove('show');
        }
    });

    // Load số lượng notification chưa đọc
    function loadUnreadNotifications() {
        fetch('/Notifications/GetUnreadCount')
            .then(response => response.json())
            .then(count => {
                unreadCount = count;
                updateNotificationBadge();
            })
            .catch(err => console.error("Error loading unread count:", err));
    }

    // Load danh sách notifications
    async function loadNotifications() {
        if (!notificationList) return;

        try {
            const response = await fetch('/Notifications/GetRecentNotifications');
            
            if (!response.ok) {
                throw new Error('Failed to load notifications');
            }

            const notifications = await response.json();

            if (notifications.length === 0) {
                notificationList.innerHTML = `
                    <div class="notification-empty">
                        <i class="fas fa-inbox"></i>
                        <p class="mb-0">Chưa có thông báo mới</p>
                    </div>
                `;
                return;
            }

            // Render notifications
            notificationList.innerHTML = notifications.map(notif => {
                const icon = getNotificationIcon(notif.type);
                const typeClass = notif.type.replace(/\s+/g, '-').toLowerCase();
                const timeAgo = getTimeAgo(new Date(notif.createdAt));

                return `
                    <div class="notification-dropdown-item ${notif.isRead ? '' : 'unread'}" 
                         onclick="handleNotificationClick(${notif.notificationId}, '${notif.link || ''}')">
                        <div class="notification-item-icon ${typeClass}">
                            ${icon}
                        </div>
                        <div class="notification-item-content">
                            <div class="notification-item-title">${escapeHtml(notif.title)}</div>
                            <div class="notification-item-message">${escapeHtml(notif.message)}</div>
                            <div class="notification-item-time">
                                <i class="far fa-clock"></i> ${timeAgo}
                            </div>
                        </div>
                        ${!notif.isRead ? '<div class="notification-item-badge"></div>' : ''}
                    </div>
                `;
            }).join('');

        } catch (error) {
            console.error('Error loading notifications:', error);
            notificationList.innerHTML = `
                <div class="notification-empty">
                    <i class="fas fa-exclamation-triangle"></i>
                    <p class="mb-0">Không thể tải thông báo</p>
                </div>
            `;
        }
    }

    // Update notification badge
    function updateNotificationBadge() {
        if (unreadCount > 0) {
            const displayCount = unreadCount > 99 ? '99+' : unreadCount;
            if (countBadge) {
                countBadge.textContent = displayCount;
                countBadge.style.display = 'block';
            }
            if (countBadgeMobile) {
                countBadgeMobile.textContent = displayCount;
                countBadgeMobile.style.display = 'block';
            }
        } else {
            if (countBadge) countBadge.style.display = 'none';
            if (countBadgeMobile) countBadgeMobile.style.display = 'none';
        }
    }

    // Hiển thị toast notification khi nhận thông báo real-time
    function showNotificationToast(notification) {
        const toastHtml = `
            <div class="toast-notification" style="
                position: fixed; 
                top: 80px; 
                right: 20px; 
                background: white; 
                padding: 15px 20px; 
                border-radius: 12px; 
                box-shadow: 0 10px 30px rgba(0,0,0,0.2);
                border-left: 4px solid ${getNotificationColor(notification.type)};
                max-width: 380px;
                animation: slideIn 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);
                z-index: 99999;">
                <div style="display: flex; align-items: start; gap: 12px;">
                    <div style="
                        width: 40px;
                        height: 40px;
                        border-radius: 50%;
                        background: linear-gradient(135deg, ${getNotificationColor(notification.type)}, ${getNotificationColor(notification.type)}99);
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        font-size: 20px;
                        flex-shrink: 0;">
                        ${getNotificationIcon(notification.type)}
                    </div>
                    <div style="flex: 1; min-width: 0;">
                        <strong style="display: block; margin-bottom: 6px; font-size: 14px; color: #1a202c;">
                            ${notification.title}
                        </strong>
                        <p style="margin: 0; color: #718096; font-size: 13px; line-height: 1.4;">
                            ${notification.message}
                        </p>
                        ${notification.link ? `
                            <a href="${notification.link}" style="
                                display: inline-block;
                                margin-top: 8px;
                                color: ${getNotificationColor(notification.type)};
                                font-size: 12px;
                                font-weight: 600;
                                text-decoration: none;">
                                Xem chi tiết →
                            </a>
                        ` : ''}
                    </div>
                    <button onclick="this.closest('.toast-notification').remove()" 
                            style="
                                border: none; 
                                background: none; 
                                cursor: pointer; 
                                color: #cbd5e0;
                                font-size: 18px;
                                padding: 0;
                                line-height: 1;
                                flex-shrink: 0;">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            </div>
        `;

        const container = document.createElement('div');
        container.innerHTML = toastHtml;
        document.body.appendChild(container.firstElementChild);

        // Auto remove sau 6 giây
        setTimeout(() => {
            const toast = document.querySelector('.toast-notification');
            if (toast) {
                toast.style.animation = 'slideOut 0.3s ease-in forwards';
                setTimeout(() => toast.remove(), 300);
            }
        }, 6000);
    }

    // Thêm notification mới vào dropdown (real-time)
    function addNotificationToDropdown(notification) {
        if (!notificationList || !dropdown.dataset.loaded) return;

        const icon = getNotificationIcon(notification.type);
        const typeClass = notification.type.replace(/\s+/g, '-').toLowerCase();

        const itemHtml = `
            <div class="notification-dropdown-item unread" 
                 onclick="handleNotificationClick(${notification.notificationId}, '${notification.link || ''}')"
                 style="animation: slideInRight 0.3s ease-out;">
                <div class="notification-item-icon ${typeClass}">
                    ${icon}
                </div>
                <div class="notification-item-content">
                    <div class="notification-item-title">${escapeHtml(notification.title)}</div>
                    <div class="notification-item-message">${escapeHtml(notification.message)}</div>
                    <div class="notification-item-time">
                        <i class="far fa-clock"></i> Vừa xong
                    </div>
                </div>
                <div class="notification-item-badge"></div>
            </div>
        `;

        // Thêm vào đầu danh sách
        notificationList.insertAdjacentHTML('afterbegin', itemHtml);
    }

    // Get notification icon based on type
    function getNotificationIcon(type) {
        const icons = {
            'Achievement': '🎉',
            'Performance Alert': '⚠️',
            'Learning Path': '💡',
            'Score Update': '📊'
        };
        return icons[type] || '📢';
    }

    // Get notification color
    function getNotificationColor(type) {
        const colors = {
            'Achievement': '#28a745',
            'Performance Alert': '#dc3545',
            'Learning Path': '#6f42c1',
            'Score Update': '#17a2b8'
        };
        return colors[type] || '#6c757d';
    }

    // Get time ago text
    function getTimeAgo(date) {
        const now = new Date();
        const diff = now - date;
        const minutes = Math.floor(diff / 60000);
        const hours = Math.floor(diff / 3600000);
        const days = Math.floor(diff / 86400000);

        if (minutes < 1) return 'Vừa xong';
        if (hours < 1) return `${minutes} phút trước`;
        if (days < 1) return `${hours} giờ trước`;
        if (days < 7) return `${days} ngày trước`;

        return date.toLocaleDateString('vi-VN');
    }

    // Escape HTML to prevent XSS
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Handle notification click (global function)
    window.handleNotificationClick = async function(notificationId, link) {
        try {
            // Mark as read
            await fetch(`/Notifications/MarkRead/${notificationId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            // Update count
            if (unreadCount > 0) {
                unreadCount--;
                updateNotificationBadge();
            }

            // Navigate to link
            if (link) {
                window.location.href = link;
            } else {
                window.location.href = '/Notifications/MyNotifications';
            }
        } catch (error) {
            console.error('Error marking notification as read:', error);
            // Still navigate even if marking fails
            if (link) {
                window.location.href = link;
            }
        }
    };

    // Load initial notification count khi page load
    loadUnreadNotifications();

    // Auto-refresh mỗi 30 giây (fallback nếu không dùng SignalR)
    setInterval(() => {
        loadUnreadNotifications();
        if (dropdown?.dataset.loaded === 'true' && dropdown.classList.contains('show')) {
            loadNotifications();
        }
    }, 30000);

    // CSS Animations
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
        @keyframes slideInRight {
            from {
                transform: translateX(-20px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
    `;
    document.head.appendChild(style);

    // Export để sử dụng ở nơi khác
    window.notificationHub = connection;
    window.reloadNotifications = loadNotifications;
    window.reloadNotificationCount = loadUnreadNotifications;
})();

