namespace SnekNet.Api.Reddit
{
    public class MeResponse
    {
        public bool is_employee { get; set; }
        public bool has_visited_new_profile { get; set; }
        public bool pref_no_profanity { get; set; }
        public bool is_suspended { get; set; }
        public string pref_geopopular { get; set; }
        public bool pref_show_trending { get; set; }
        public object subreddit { get; set; }
        public bool is_sponsor { get; set; }
        public object gold_expiration { get; set; }
        public string id { get; set; }
        public object suspension_expiration_utc { get; set; }
        public bool verified { get; set; }
        public Features features { get; set; }
        public bool over_18 { get; set; }
        public bool is_gold { get; set; }
        public bool is_mod { get; set; }
        public bool has_verified_email { get; set; }
        public bool in_redesign_beta { get; set; }
        public string icon_img { get; set; }
        public string oauth_client_id { get; set; }
        public bool hide_from_robots { get; set; }
        public int link_karma { get; set; }
        public int inbox_count { get; set; }
        public object pref_top_karma_subreddits { get; set; }
        public bool pref_show_snoovatar { get; set; }
        public string name { get; set; }
        public int pref_clickgadget { get; set; }
        public float created { get; set; }
        public int gold_creddits { get; set; }
        public float created_utc { get; set; }
        public bool in_beta { get; set; }
        public int comment_karma { get; set; }
        public bool has_subscribed { get; set; }
    }

    public class Features
    {
        public bool adzerk_reporting_2 { get; set; }
        public bool listing_service_rampup { get; set; }
        public bool ad_moderation { get; set; }
        public bool mweb_xpromo_modal_listing_click_daily_dismissible_ios { get; set; }
        public bool show_amp_link { get; set; }
        public bool native_ad_server { get; set; }
        public bool live_happening_now { get; set; }
        public bool adserver_reporting { get; set; }
        public bool geopopular { get; set; }
        public bool search_public_traffic { get; set; }
        public bool chat_rollout { get; set; }
        public bool ads_auto_refund { get; set; }
        public bool chat { get; set; }
        public bool archived_signup_cta { get; set; }
        public bool mobile_web_targeting { get; set; }
        public bool rte_video { get; set; }
        public bool ads_auction { get; set; }
        public bool adzerk_do_not_track { get; set; }
        public bool loadtest_sendbird_me { get; set; }
        public bool users_listing { get; set; }
        public bool mweb_xpromo_modal_listing_click_daily_dismissible_link { get; set; }
        public bool whitelisted_pms { get; set; }
        public Default_Srs_Holdout default_srs_holdout { get; set; }
        public bool circleoftrust_livevotes { get; set; }
        public bool personalization_prefs { get; set; }
        public bool upgrade_cookies { get; set; }
        public bool new_overview { get; set; }
        public bool block_user_by_report { get; set; }
        public bool legacy_search_pref { get; set; }
        public bool orangereds_as_emails { get; set; }
        public bool do_not_track { get; set; }
        public bool ios_profile_edit { get; set; }
        public bool crossposting_recent { get; set; }
        public bool heartbeat_events { get; set; }
        public bool expando_events { get; set; }
        public bool eu_cookie_policy { get; set; }
        public bool programmatic_ads { get; set; }
        public bool force_https { get; set; }
        public bool activity_service_write { get; set; }
        public bool chat_menu_notification { get; set; }
        public bool post_to_profile_beta { get; set; }
        public bool crossposting_ga { get; set; }
        public bool profile_redesign { get; set; }
        public bool outbound_clicktracking { get; set; }
        public bool show_user_sr_name { get; set; }
        public bool new_loggedin_cache_policy { get; set; }
        public bool interest_targeting { get; set; }
        public bool user_otp { get; set; }
        public bool https_redirect { get; set; }
        public bool screenview_events { get; set; }
        public bool pause_ads { get; set; }
        public Logistic_Regression_V2 logistic_regression_v2 { get; set; }
        public bool mweb_xpromo_modal_listing_click_daily_dismissible_android { get; set; }
        public bool give_hsts_grants { get; set; }
        public bool mobile_native_banner { get; set; }
        public bool mweb_xpromo_interstitial_comments_android { get; set; }
        public bool ios_promoted_posts { get; set; }
        public bool mweb_xpromo_interstitial_comments_ios { get; set; }
        public bool moat_tracking { get; set; }
        public bool subreddit_rules { get; set; }
        public Mweb_Xpromo_Incognito_Noxpromo mweb_xpromo_incognito_noxpromo { get; set; }
        public bool inbox_push { get; set; }
        public bool ads_auto_extend { get; set; }
        public bool onboarding_splash2 { get; set; }
        public bool post_embed { get; set; }
        public bool mobile_ad_image { get; set; }
        public bool chat_group_rollout { get; set; }
        public bool circle_pm { get; set; }
        public bool scroll_events { get; set; }
        public Search_Comment_Faceting search_comment_faceting { get; set; }
        public bool activity_service_read { get; set; }
        public bool adblock_test { get; set; }
        public bool redesign_beta { get; set; }
    }

    public class Default_Srs_Holdout
    {
        public string owner { get; set; }
        public string variant { get; set; }
        public int experiment_id { get; set; }
    }

    public class Logistic_Regression_V2
    {
        public string owner { get; set; }
        public string variant { get; set; }
        public int experiment_id { get; set; }
    }

    public class Mweb_Xpromo_Incognito_Noxpromo
    {
        public string owner { get; set; }
        public string variant { get; set; }
        public int experiment_id { get; set; }
    }

    public class Search_Comment_Faceting
    {
        public string owner { get; set; }
        public string variant { get; set; }
        public int experiment_id { get; set; }
    }
}
