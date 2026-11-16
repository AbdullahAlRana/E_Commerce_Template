namespace ECommerce.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Refunded = 6
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3
}

public enum PaymentMethod
{
    CashOnDelivery = 0,
    CreditCard = 1,
    DebitCard = 2,
    PayPal = 3,
    Stripe = 4
}

public enum CouponType
{
    Percentage = 0,
    FixedAmount = 1
}

public enum UserRole
{
    Customer = 0,
    Admin = 1
}