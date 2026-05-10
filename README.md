# 🎮 DV Match-3 Game

Chào mừng bạn đến với **DV Match-3 Game**! Đây là một dự án game giải đố Match-3 hoàn chỉnh được phát triển trên Unity. Dự án được cấu trúc bài bản với các design pattern chuyên nghiệp, đem lại hiệu suất cao, khả năng mở rộng tốt và trải nghiệm mượt mà.

## 🌟 Các Tính Năng Nổi Bật (Features)

* **Gameplay Cốt Lõi**:
  * Chu trình vòng lặp trò chơi chuẩn xác: Swap (Đổi) -> Match (Ghép) -> Fall (Rơi) -> Fill (Lấp đầy).
  * Hỗ trợ tìm kiếm combo, xử lý nhiều lượt match liên tiếp một cách ổn định và chính xác (thông qua mảng bitboard và các service).
* **Luật Rơi Chéo (Diagonal Fall)**: Thay vì chỉ rơi dọc đơn thuần, hệ thống tự động quét và kéo kẹo rơi chéo xuống các ô trống kế cận khi phía trên bị chặn bởi chướng ngại vật, tạo cảm giác linh hoạt và logic vật lý chân thực.
* **Cơ Chế Kẹo Đặc Biệt (Special Candies)**:
  * **Line Candies (Ngang/Dọc)**: Quét sạch một hàng hoặc một cột.
  * **Bomb Candies**: Kích hoạt vụ nổ phá hủy khu vực xung quanh (T/L shape matches).
  * **Color Bombs**: Xóa toàn bộ kẹo có cùng màu trên màn hình (Match 5 liên tiếp).
* **Chướng Ngại Vật (Obstacles)**:
  * Hệ thống gạch (Bricks) ngăn cản kẹo di chuyển và rơi xuống. Cải tiến logic ngăn chặn người chơi tráo đổi (swap) kẹo thường với gạch.
* **Hình Ảnh & Hiệu Ứng (Polish)**:
  * Tích hợp **DOTween** cho các hiệu ứng tráo đổi (Swap) mượt mà, tạo cảm giác thỏa mãn (juicy) cho người chơi.
* **Hệ Thống UI & Âm Thanh**:
  * Quản lý cài đặt âm thanh (SFX, Music) trực quan bằng Slider, khắc phục triệt để lỗi phát lại âm thanh liên tục khi kéo thanh trượt hoặc kéo về giá trị 0.

## 🏗️ Kiến Trúc Mã Nguồn (Architecture)

Dự án áp dụng mô hình kiến trúc có tính module hóa cao (Service/Controller/State Machine) nhằm đảm bảo tiêu chuẩn Portfolio của Unity Developer.

* **State Machine Pattern (`Assets/_Data/Scripts/States/`)**:
  * Vòng lặp trò chơi được kiểm soát chặt chẽ bởi các trạng thái độc lập, đảm bảo tuần tự, tránh tình trạng bất đồng bộ hay đè chéo logic:
    * `StartState`: Khởi tạo bảng.
    * `IdleState`: Chờ tương tác từ người chơi.
    * `SwapState`: Xử lý di chuyển hai viên kẹo (áp dụng DOTween).
    * `FallState`: Kéo kẹo xuống vị trí trống (bao gồm thuật toán rơi chéo).
    * `FillState`: Sinh ra kẹo mới để lấp đầy bảng.
* **Service - Controller Pattern (`Assets/_Data/Scripts/Controllers/` & `Services/`)**:
  * `BoardController` & `CandyController`: Quản lý hiển thị và tương tác của GameObjects (View).
  * `BoardService` & `MatchService`: Xử lý lõi thuật toán (Model/Logic), đồng bộ song song dữ liệu Grid (GameObject) và Bitboard Data (Mảng int) để tính toán lượt match một cách tối ưu, tránh lỗi NullReference.
* **Enums (`Assets/_Data/Scripts/Enum/`)**:
  * `CandyEnum`: Định nghĩa rõ ràng ID cho tất cả các loại kẹo (Normal, Horizontal, Vertical, Bomb, ColorBomb).
  * `SpecialEnum`: Các trạng thái kích hoạt đặc biệt, giúp code sạch hơn thay vì lạm dụng Magic Numbers.

## 🚀 Hướng Dẫn Cài Đặt (Getting Started)

1. **Yêu cầu hệ thống**:
   * Unity Editor (Khuyên dùng bản cài đặt hiện tại của dự án hoặc các phiên bản Unity LTS gần nhất).
   * Tích hợp sẵn package **DOTween** (Được quản lý thông qua `DoTweenManager.cs`).
2. **Clone & Mở Dự Án**:
   ```bash
   git clone <repository_url>
   ```
   Mở thư mục `dv_match3` bằng Unity Hub.
3. **Chạy Game**:
   * Mở Scene chính (nằm trong thư mục `Assets`).
   * Nhấn nút **Play** trên Unity Editor để trải nghiệm trực tiếp!

## 🤝 Định Hướng Phát Triển Tiếp Theo

* Thêm hệ thống Level và mục tiêu màn chơi (Score, Time, thu thập một số lượng kẹo cụ thể).
* Mở rộng thêm các cơ chế bổ trợ (Boosters / Power-ups) ngoài màn hình.
* Cải thiện hệ thống hạt (Particle Systems) cho các chuỗi combo và vụ nổ lớn.