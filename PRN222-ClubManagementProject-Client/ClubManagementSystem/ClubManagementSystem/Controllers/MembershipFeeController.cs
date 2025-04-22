using BussinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interface;
using System.Security.Claims;
using Repositories.Interface;
using ClubManagementSystem.Services;

namespace ClubManagementSystem.Controllers
{
    public class MembershipFeeController : Controller
    {
        private readonly IMembershipFeeService _membershipFeeService;
        private readonly IMembershipFeeRepository _membershipFeeRepository;
        private readonly IVnPayService _vnPayService;
        private readonly IQueueService _queueService;


        public MembershipFeeController(IMembershipFeeService membershipFeeService,   IMembershipFeeRepository membershipFeeRepository,  
            IVnPayService vnPayService, IQueueService queueService)
        {
            _membershipFeeService = membershipFeeService;
            _membershipFeeRepository = membershipFeeRepository;
            _vnPayService = vnPayService;
            _queueService = queueService;
        }

        public async Task<IActionResult> Index(int clubId)
        {
            var fees = await _membershipFeeService.GetFeesByClubAsync(clubId);
            if (fees == null)
            {
                return NotFound("Không tìm thấy danh sách phí.");
            }
            ViewBag.ClubId = clubId;
            return View(fees);
        }

        // GET: Hiển thị form để chỉ tạo Fee
        public IActionResult CreateFee(int clubId)
        {
            ViewBag.ClubId = clubId;
            return View();
        }

        // POST: Chỉ tạo Fee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFee(int clubId, decimal amount, DateTime dueDate, string feeDescription, string feeType)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClubId = clubId;
                return View();
            }

            try
            {
                var result = await _membershipFeeService.CreateFeeAsync(clubId, amount, dueDate, feeDescription, feeType);

                if (!result)
                {
                    ModelState.AddModelError("", "Không thể tạo phí. Câu lạc bộ không tồn tại.");
                    ViewBag.ClubId = clubId;
                    return View();
                }

                return RedirectToAction("Index", new { clubId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo phí: " + ex.Message);
                ViewBag.ClubId = clubId;
                return View();
            }
        }

        // GET: Hiển thị form để áp dụng Fee cho tất cả thành viên
        public async Task<IActionResult> ApplyFee(int clubId)
        {
            var fees = await _membershipFeeService.GetFeesByClubAsync(clubId);
            ViewBag.ClubId = clubId;
            ViewBag.Fees = fees;
            return View();
        }

        // POST: Áp dụng Fee cho tất cả thành viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyFee(int clubId, int feeId)
        {
            try
            {
                var result = await _membershipFeeService.ApplyFeeToAllMembersAsync(clubId, feeId);

                if (!result)
                {
                    var fees = await _membershipFeeService.GetFeesByClubAsync(clubId);
                    ViewBag.ClubId = clubId;
                    ViewBag.Fees = fees;
                    ModelState.AddModelError("", "Không thể áp dụng phí. Kiểm tra câu lạc bộ hoặc phí.");
                    return View();
                }

                return RedirectToAction("Index", new { clubId });
            }
            catch (Exception ex)
            {
                var fees = await _membershipFeeService.GetFeesByClubAsync(clubId);
                ViewBag.ClubId = clubId;
                ViewBag.Fees = fees;
                ModelState.AddModelError("", "Có lỗi xảy ra khi áp dụng phí: " + ex.Message);
                return View();
            }
        }

        // GET: Hiển thị form để tạo phí và áp dụng ngay cho tất cả thành viên
        public IActionResult Create(int clubId)
        {
            ViewBag.ClubId = clubId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int clubId, decimal amount, DateTime dueDate, string feeDescription, string feeType, string paymentMethod)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClubId = clubId;
                return View();
            }

            try
            {
                var result = await _membershipFeeService.CreateMembershipFeesForClubAsync(clubId, amount, dueDate, feeDescription, feeType, paymentMethod);

                if (!result)
                {
                    ModelState.AddModelError("", "Không thể tạo phí. Câu lạc bộ không tồn tại hoặc không có thành viên.");
                    ViewBag.ClubId = clubId;
                    return View();
                }

                return RedirectToAction("Index", new { clubId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo phí: " + ex.Message);
                ViewBag.ClubId = clubId;
                return View();
            }
        }

        public async Task<IActionResult> Edit(int feeId)
        {
            var fee = await _membershipFeeService.GetFeeByIdAsync(feeId);
            if (fee == null)
            {
                return NotFound("Không tìm thấy phí để chỉnh sửa.");
            }
            ViewBag.FeeTypes = new SelectList(new[]
                {
                new { Value = "Membership", Text = "Phí Thành Viên" },
                new { Value = "Event", Text = "Phí Sự Kiện" },
                new { Value = "Penalty", Text = "Phí Phạt" }
                }, "Value", "Text", fee.FeeType);
            return View(fee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int feeId, decimal amount, DateTime dueDate, string feeDescription, string feeType, string paymentMethod)
        {
            if (!ModelState.IsValid)
            {
                var fee = await _membershipFeeService.GetFeeByIdAsync(feeId);
                return View(fee);
            }

            try
            {
                var result = await _membershipFeeService.UpdateFeeAsync(feeId, amount, dueDate, feeDescription, feeType, paymentMethod);

                if (!result)
                {
                    return NotFound("Không thể cập nhật phí.");
                }

                var fee = await _membershipFeeService.GetFeeByIdAsync(feeId);
                return RedirectToAction("Index", new { clubId = fee.ClubId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật phí: " + ex.Message);
                var fee = await _membershipFeeService.GetFeeByIdAsync(feeId);
                return View(fee);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int clubId, int feeId)
        {
            try
            {
                var result = await _membershipFeeService.DeleteFeeAndRelatedFeesAsync(feeId);

                if (!result)
                {
                    return NotFound("Không tìm thấy phí để xóa.");
                }

                return RedirectToAction("Index", new { clubId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa phí: " + ex.Message;
                return RedirectToAction("Index", new { clubId });
            }
        }

        // GET: Hiển thị danh sách thành viên cần nộp phí
        public async Task<IActionResult> Details(int feeId)
        {
            var membershipFees = await _membershipFeeService.GetMembershipFeesByFeeIdAsync(feeId);
            if (membershipFees == null || !membershipFees.Any())
            {
                return NotFound("Không tìm thấy thông tin phí hoặc chưa áp dụng cho thành viên nào.");
            }

            var fee = await _membershipFeeService.GetFeeByIdAsync(feeId);
            ViewBag.Fee = fee;
            return View(membershipFees);
        }

        // POST: Gửi thông báo nhắc nhở
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remind(int feeId, List<int> memberIds)
        {
            if (memberIds == null || !memberIds.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một thành viên để nhắc nhở.";
                return RedirectToAction("Details", new { feeId });
            }

            try
            {
                List<MembershipFee> result = await _membershipFeeService.RemindMembersAsync(feeId, memberIds);

                if (result==null)
                {
                    TempData["ErrorMessage"] = "Không thể gửi nhắc nhở. Kiểm tra thông tin phí hoặc thành viên.";
                    return RedirectToAction("Details", new { feeId });
                }
                foreach (var member in result)
                {
                    var user = member.ClubMember?.User;
                    if (user != null)
                    {
                        _queueService.EnqueueEmail(
                            user.Email, // Assuming User has an Email property
                            user.Username, // Assuming User has a FullName property
                            "remind"
                        );
                    }
                }

                TempData["SuccessMessage"] = "Đã gửi thông báo nhắc nhở thành công.";
                return RedirectToAction("Details", new { feeId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi nhắc nhở: " + ex.Message;
                return RedirectToAction("Details", new { feeId });
            }
        }

        public async Task<IActionResult> MyFees()
        {
            // Lấy UserId của người dùng hiện tại
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Không thể xác định người dùng. Vui lòng đăng nhập lại.");
            }

            var membershipFees = await _membershipFeeService.GetMembershipFeesByUserIdAsync(userId);
            
            if (membershipFees == null || !membershipFees.Any())
            {
                ViewBag.Message = "Bạn hiện không có khoản phí nào cần đóng.";
            }

            return View(membershipFees);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayFee(int membershipFeeId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Không thể xác định người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction("MyFees");
            }

            try
            {
                var membershipFee = await _membershipFeeRepository.GetMembershipFeeByIdAsync(membershipFeeId, userId);
                if (membershipFee == null || (membershipFee.Status != "Pending" && membershipFee.Status != "Overdue"))
                {
                    TempData["ErrorMessage"] = "Không thể thanh toán khoản phí. Kiểm tra trạng thái hoặc quyền truy cập.";
                    return RedirectToAction("MyFees");
                }
                
                var model = new Models.VnPay.PaymentInformationModel
                {
                    OrderType = "other",
                    MembershipFeeId = membershipFee.MembershipFeeId,
                    Amount = membershipFee.Fee.Amount,
                    OrderDescription = $"Thanh toán phí : {membershipFee.Fee.FeeDescription}",
                    Name = "Thanh toán phí thành viên"
                };
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

                return Redirect(url);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thanh toán: " + ex.Message;
                return RedirectToAction("MyFees");
            }
        }

        public async Task<IActionResult> PaymentCallback()
        {

            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.VnPayResponseCode == "00")
            {
                int membershipFeeId = response.OrderDescription;

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    TempData["ErrorMessage"] = "Không thể xác định người dùng.";
                    return RedirectToAction("MyFees");
                }

                var membershipFee = await _membershipFeeRepository.GetMembershipFeeByIdAsync(membershipFeeId, userId);
                if (membershipFee != null && (membershipFee.Status == "Pending" || membershipFee.Status == "Overdue"))
                {
                    membershipFee.Status = "Paid";
                    membershipFee.PaidAt = DateTime.Now;
                    await _membershipFeeRepository.UpdateMembershipFeeAsync(membershipFee);

                    var notification = new Notification
                    {
                        UserId = userId,
                        Message = $"Bạn đã thanh toán thành công khoản phí: {membershipFee.Fee.FeeDescription} - {membershipFee.Fee.Amount} VND.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _membershipFeeRepository.AddNotificationAsync(notification);
                    _queueService.EnqueueEmail(
                            User.FindFirst(ClaimTypes.Email)?.Value, // Assuming User has an Email property
                            User.FindFirst(ClaimTypes.Name)?.Value, // Assuming User has a FullName property
                            "payment"
                        );
                }

                TempData["SuccessMessagePayment"] = "Payment Success!";
            }
            else
            {
                TempData["ErrorMessagePayment"] = "Payment Failed!";
            }

            return RedirectToAction("MyFees");
        }
    }
}
