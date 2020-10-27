var xhr;
var isDeityInput = document.getElementById('is-deity-flag');

var my_autoComplete = new autoComplete({
  selector: '.search__search-form__input',
  source: function (term, response) {
    var searchSuggestionsUrl = '/search/suggestions/' + term;

    try {
      xhr.abort();
    } catch (e) {
      console.log(e); }

    xhr = $.post(searchSuggestionsUrl, function (data) {
      response(data.matches);
    });
  },
  renderItem: function (item, search) {
    search = search.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
    var regularExpression = new RegExp("(" + search.split(' ').join('|') + ")", "gi");
    var type = 'Deity';
    if (item.isDeity === false) {
      type = 'Belief System';
    }
    return '<div class="autocomplete-suggestion" data-val="' + item.slug + '" data-isdeity="' + item.isDeity + '">' + item.name.replace(regularExpression, "<b>$1</b>") + ' - <em>' + type + '</em></div>';
  },
  onSelect: function (e, term, item) {
    console.log('Item is: ');
    console.log(item);

    isDeityInput.value = item.getAttribute('data-isdeity');
  }
});
